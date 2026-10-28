# Retrieval eval results

Setup (shared by all runs below unless stated otherwise):
- Corpus: 20 documents from `seed-data/` → 62 chunks (`ChunkingService`, 500 tokens, 50 overlap)
- Embedding model: `qwen3-embedding:0.6b` (Ollama), cosine distance in pgvector
- Dataset: `Data/retrieval-set.json` — 97 cases (85 in-domain, 12 out-of-domain; 52 PL / 45 EN)
- Baseline: `topK = 10`, no distance threshold
- Sweep: `topK = 6` (production value), `maxDistance` = threshold

Metrics: recall@k = share of in-domain questions with the expected document in the top k; MRR = mean of 1/rank (0 when missing); OOD rejected = share of out-of-domain questions that got an empty result.

---

## 2026-10-28 — Run 1: no query prefix

### Baseline

| recall@1 | recall@3 | recall@6 | recall@10 | MRR |
|---|---|---|---|---|
| 0.85 | 0.92 | 1.00 | 1.00 | 0.90 |

### Threshold sweep

| maxDistance | recall@6 | MRR | OOD rejected |
|---|---|---|---|
| 0.60 | 0.73 | – | 1.00 |
| 0.65 | 0.87 | – | 1.00 |
| 0.70 | 0.95 | 0.88 | 1.00 |
| 0.72 | 0.96 | 0.88 | 0.92 |
| 0.74 | 0.98 | 0.88 | 0.83 |
| 0.76 | 0.99 | 0.90 | 0.75 |
| 0.78 | 0.99 | 0.90 | 0.58 |
| 0.80 | 0.99 | 0.90 | 0.50 |
| 0.82 | 0.99 | 0.90 | 0.17 |
| 0.84 | 0.99 | 0.90 | 0.08 |
| 0.86 | 1.00 | 0.90 | 0.00 |
| 0.88 | 1.00 | 0.90 | 0.00 |
| 0.90 | 1.00 | 0.90 | 0.00 |

---

## 2026-10-28 — Run 2: qwen3 query instruction prefix

Query text sent to the embedding model (documents are embedded without a prefix):

```
Instruct: Given a web search query, retrieve relevant passages that answer the query
Query: <question>
```

### Baseline

| recall@1 | recall@3 | recall@6 | recall@10 | MRR |
|---|---|---|---|---|
| 0.88 | 0.95 | 1.00 | 1.00 | 0.93 |

### Threshold sweep

| maxDistance | recall@6 | MRR | OOD rejected |
|---|---|---|---|
| 0.60 | 0.93 | 0.87 | 1.00 |
| 0.62 | 0.96 | 0.91 | 1.00 |
| 0.64 | 0.96 | 0.91 | 1.00 |
| 0.66 | 0.96 | 0.91 | 1.00 |
| **0.68** | **0.98** | **0.92** | **1.00** |
| 0.70 | 0.99 | 0.92 | 0.92 |
| 0.72 | 0.99 | 0.92 | 0.92 |
| 0.74 | 0.99 | 0.92 | 0.83 |
| 0.76 | 0.99 | 0.92 | 0.58 |
| 0.78 | 0.99 | 0.92 | 0.42 |
| 0.80 | 1.00 | 0.93 | 0.33 |
| 0.82 | 1.00 | 0.93 | 0.33 |
| 0.84 | 1.00 | 0.93 | 0.17 |
| 0.86 | 1.00 | 0.93 | 0.00 |
| 0.88 | 1.00 | 0.93 | 0.00 |
| 0.90 | 1.00 | 0.93 | 0.00 |

### Conclusion

At zero out-of-domain leakage (OOD rejected = 1.00) the prefix raises recall@6 from 0.95 (threshold 0.70, no prefix) to 0.98 (threshold 0.68, with prefix) and MRR from 0.88 to 0.92. Baseline recall@1 improves 0.85 → 0.88.

Chosen configuration: **query prefix + `maxDistance = 0.68`**.

Caveats:
- Thresholds are not comparable between runs — the prefix shifts all distances; compare rows with the same OOD rejection rate.
- Only 12 out-of-domain cases; the nearest one lies between 0.68 and 0.70, so the margin is narrow. Add more near-domain OOD questions before trusting the threshold.
- The threshold was tuned on the same dataset it is measured on, so real-world recall will be slightly lower.
