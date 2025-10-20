# Job Application Guide - Junior .NET Developer

This guide covers improvements needed for your CV and preparation for applying to your first .NET Developer position.

---

## Current CV Status

### Strengths ✅
- Clean, professional layout with photo
- Three real projects showing progression
- Relevant education (ongoing engineering studies in programming)
- Good language skills (Polish native, English B2)
- Modern technologies mentioned (Clean Architecture, DDD, Modular Monolith)
- Active learning (courses: Modular Monolith DevMentors, SOLID WEB API DevMentors)

### Areas to Improve 🔧
- Missing your most impressive project (MealMind)
- Skills ratings are conservative (you're better than you think!)
- Profile section could be more confident
- Missing some key technologies you actually use (PostgreSQL, Docker, CQRS)

---

## 1. Add MealMind Project (PRIORITY #1)

### Where to Add
Add as the **first project** in your "Projekty" section, above EduSchedu.

### Template to Use

```markdown
MealMind
10/2024–Obecnie

System zarządzania żywieniem i AI chat z RAG – aplikacja backendowa.

Modularny monolit (.NET 9) do śledzenia żywienia użytkowników z
integracją AI chatbota wykorzystującego RAG (Retrieval-Augmented Generation).
Aplikacja składa się z trzech modułów: Identity (autentykacja), Nutrition
(profile żywieniowe, cele makroskładników), oraz AiChat (konwersacyjne AI
z wyszukiwaniem semantycznym).

System wykorzystuje:
• Clean Architecture i DDD (agregaty, value objects, domain events)
• CQRS z MediatR do separacji komend i zapytań
• PostgreSQL z rozszerzeniem pgvector do przechowywania embeddingów
• Ollama (llama3.2:3b) i Semantic Kernel do lokalnego LLM
• RAG z 768-wymiarowymi wektorami dla semantycznego wyszukiwania
• Docker dla PostgreSQL i Ollama
• Entity Framework Core z migracjami

Technologie użyte w projekcie: .NET 9, C#, PostgreSQL, pgvector,
Entity Framework Core, MediatR, Semantic Kernel, Ollama, Docker,
Clean Architecture, DDD, CQRS.
```

### Why This Matters
- Shows you're actively developing (10/2024–Obecnie)
- Demonstrates advanced concepts (RAG, AI, vector embeddings)
- Proves you can work with modern tech stack
- Shows initiative and passion for learning

---

## 2. Update Skills Section

### Current Skills (Conservative)
```
C# ................................................... 3/5
.NET ................................................. 2/5
EF Core .............................................. 2/5
Entity Framework ..................................... 2/5
LINQ ................................................. 2/5
Clean Architecture ................................... 2/5
Modular Monolith ..................................... 2/5
DDD ................................................... 1/5
Python ................................................ 1/5
```

### Recommended Updated Skills
```
C# .................................................... 4/5  ⬆️ (You built 3 projects!)
.NET .................................................. 3/5  ⬆️ (Using .NET 9)
EF Core .............................................. 3/5  ⬆️ (Migrations, configurations)
Entity Framework ..................................... 2/5  (Keep as is)
LINQ ................................................. 3/5  ⬆️ (Using daily)
Clean Architecture ................................... 3/5  ⬆️ (3 projects with it)
Modular Monolith ..................................... 3/5  ⬆️ (2 projects)
DDD ................................................... 3/5  ⬆️ (Aggregates, value objects)
CQRS .................................................. 2/5  🆕 (MealMind uses it)
PostgreSQL ............................................ 3/5  🆕 (With pgvector!)
Docker ................................................ 2/5  🆕 (Docker compose)
Git ................................................... 3/5  🆕 (Version control)
Python ................................................ 1/5  (Keep as is)
```

### Why These Ratings?
- **1/5** = Basic knowledge, used in tutorials
- **2/5** = Can use with documentation, understand fundamentals
- **3/5** = Comfortable using, built real projects with it
- **4/5** = Very comfortable, can solve complex problems
- **5/5** = Expert level (rarely appropriate for junior)

**For Junior positions, 3/5 is excellent. It shows competence without overconfidence.**

---

## 3. Improve Profile Section

### Current Profile (Good but timid)
```
Jestem studentem na studiach inżynieryjnych na kierunku programowanie.
Specjalizuję się w projektowaniu i implementacji systemów backendowych z
wykorzystaniem technologii takich jak ASP.NET Core oraz wzorców takich jak DDD
(Domain-Driven Design), Clean Architecture i Modular Monolith. Nie posiadam
żadnego komercyjnego doświadczenia, ale szukam pracy, w której mógłbym rozwinąć
swoją pasję oraz zainteresowania związane z tworzeniem oprogramowania.
```

**Issues:**
- ❌ "Nie posiadam żadnego komercyjnego doświadczenia" - Too negative
- ❌ Doesn't mention your most impressive work (MealMind)
- ❌ Sounds unsure ("mógłbym")

### Improved Profile (Confident but honest)
```
Jestem studentem inżynierii na kierunku programowanie, specjalizującym
się w projektowaniu i implementacji systemów backendowych z wykorzystaniem
ASP.NET Core. Posiadam doświadczenie w tworzeniu aplikacji z wykorzystaniem
wzorców takich jak DDD (Domain-Driven Design), Clean Architecture, Modular
Monolith oraz CQRS.

Aktualnie rozwijam projekt MealMind - modularny system żywieniowy z
integracją AI wykorzystujący RAG i lokalne modele językowe (LLM).

Szukam pierwszej pracy komercyjnej, w której mógłbym rozwijać swoje
umiejętności oraz przyczyniać się do tworzenia wartościowego oprogramowania.
```

**Why This Is Better:**
- ✅ Confident opening
- ✅ Highlights current work (MealMind)
- ✅ Shows advanced skills (AI, RAG, LLM)
- ✅ Honest about seeking first commercial role
- ✅ Focus on value you can bring

---

## 4. Add Technologies & Tools Section

Add a new section between "Umiejętności" and "Języki":

```
Narzędzia i Technologie

Backend: ASP.NET Core, C#, Entity Framework Core, MediatR, LINQ
Bazy danych: PostgreSQL, pgvector, SQL Server
AI/ML: Ollama, Semantic Kernel, RAG, Vector Embeddings
Architektura: Clean Architecture, DDD, CQRS, Modular Monolith
Narzędzia: Git, Docker, Visual Studio Code, Rider
Testing: TUnit (po dodaniu testów)
```

**Why Add This:**
- Makes it easy for recruiters to scan
- Shows breadth of knowledge
- Keywords for ATS (Applicant Tracking Systems)
- Demonstrates you're current with modern tools

---

## 5. Polish Your GitHub Profile

### Before Applying, Make Sure:

#### Your MealMind Repository Has:
- ✅ **Professional README.md** with:
  - Project description
  - Architecture diagram or explanation
  - Technologies used
  - How to run locally
  - Screenshots (optional but nice)

- ✅ **Clean commit history**:
  - Descriptive commit messages
  - Not too many "fix" or "wip" commits
  - Shows progression of features

- ✅ **Good code organization**:
  - Clear folder structure
  - CLAUDE.md (your architecture guide)
  - .gitignore properly configured

#### README.md Template for MealMind:

```markdown
# MealMind

A modular nutrition tracking and AI chat platform built with .NET 9 and Clean Architecture.

## Overview

MealMind is a modular monolith that combines nutrition tracking with an AI-powered chat assistant using RAG (Retrieval-Augmented Generation). The system helps users track their nutrition goals and provides intelligent, context-aware advice.

## Architecture

The application follows Clean Architecture principles with three main modules:

- **Identity Module**: User authentication and management
- **Nutrition Module**: User profiles, personal data, and nutrition tracking
- **AiChat Module**: Conversational AI with RAG semantic search

### Tech Stack

- .NET 9 with ASP.NET Core
- PostgreSQL with pgvector extension
- Entity Framework Core
- MediatR (CQRS pattern)
- Ollama (llama3.2:3b) for local LLM
- Semantic Kernel for AI integration
- Docker for containerization

### Design Patterns

- Clean Architecture (Domain, Application, Infrastructure, API layers)
- Domain-Driven Design (Aggregates, Value Objects, Domain Events)
- CQRS (Command Query Responsibility Segregation)
- Repository Pattern
- Modular Monolith

## Features

### Nutrition Module
- User profile management
- Personal data tracking (weight, height, activity level)
- Nutrition targets (calories, macros, water intake)

### AiChat Module
- RAG-powered semantic search (768-dimensional embeddings)
- Document chunking and embedding
- Conversation context management
- Integration with local LLM (Ollama)

## Running Locally

### Prerequisites
- .NET 9 SDK
- Docker and Docker Compose
- Ollama (for LLM)

### Steps

1. Clone the repository
```bash
git clone https://github.com/yourusername/MealMind.git
cd MealMind
```

2. Start dependencies
```bash
docker-compose up -d
```

3. Pull Ollama models
```bash
docker exec -it MealMind.Ollama ollama pull llama3.2:3b
docker exec -it MealMind.Ollama ollama pull nomic-embed-text
```

4. Run migrations
```bash
dotnet ef database update --project src/Modules/Nutrition/MealMind.Modules.Nutrition.Infrastructure --startup-project src/Bootstrapper/MealMind.Bootstrapper
```

5. Run the application
```bash
dotnet run --project src/Bootstrapper/MealMind.Bootstrapper
```

## Learning Resources

See `/LearningMaterials` for detailed documentation on:
- RAG chunking strategies
- Document storage approaches
- Testing guide (TUnit)
- Architecture decisions

## Future Improvements

- [ ] Frontend UI (Blazor or React)
- [ ] Comprehensive test suite (unit, integration, E2E)
- [ ] Azure deployment
- [ ] OpenFoodFacts API integration
- [ ] Meal planning features

## License

MIT License - See LICENSE file for details
```

---

## 6. Quick Wins Before Applying (1-2 Weeks)

### Priority 1: CV Updates (1 day)
- ✅ Add MealMind project
- ✅ Update skills ratings
- ✅ Improve profile section
- ✅ Add technologies section

### Priority 2: GitHub Polish (1-2 days)
- ✅ Write professional README for MealMind
- ✅ Clean up commit messages if needed
- ✅ Ensure CLAUDE.md is up to date
- ✅ Add LICENSE file (MIT recommended)

### Priority 3: Add Basic Tests (3-5 days)
Even 5-10 tests show you understand testing principles.

**Focus on easy wins:**
1. Domain entity tests (NutritionTarget, UserProfile)
2. Value object tests (Email, Password validation)
3. Simple business logic tests

**Example:**
```csharp
public class NutritionTargetTests
{
    [Test]
    public async Task Create_ValidInputs_CreatesTarget()
    {
        // Arrange
        var calories = 2000m;
        var protein = 150m;
        var userId = Guid.NewGuid();

        // Act
        var target = NutritionTarget.Create(calories, protein, 250m, 65m, 2.5m, userId);

        // Assert
        await Assert.That(target.Calories).IsEqualTo(calories);
        await Assert.That(target.Protein).IsEqualTo(protein);
        await Assert.That(target.IsActive).IsTrue();
    }
}
```

Once you have tests, add to CV skills:
```
TUnit/Testing ........................................ 2/5
```

### Priority 4: Study Frontend Basics (1 week)
The job posting mentions "orientujesz się w technologiach frontendowych"

**Study these basics:**
- How React works (components, hooks, state)
- How frontend communicates with backend (REST APIs, fetch, axios)
- Basic HTML/CSS concepts
- What CORS is and why it matters

**Resources:**
- React docs: https://react.dev/learn
- MDN Web Docs: https://developer.mozilla.org/en-US/docs/Learn

**Optional but impressive:**
- Add simple Blazor frontend to MealMind (stays in .NET ecosystem)
- Or minimal React app that calls your API

---

## 7. Interview Preparation

### Common Questions You'll Get

#### 1. "Tell me about yourself"
**Bad answer:** "I'm a student looking for my first job..."

**Good answer:**
```
I'm an engineering student specializing in backend development with .NET.
I'm passionate about clean code and modern architecture patterns like DDD
and CQRS. Currently, I'm working on MealMind - a modular nutrition tracking
platform with AI chat capabilities using RAG. I've also completed two other
projects: EduSchedu (school management system) and TaskManager (task tracking
app), both using Clean Architecture and Modular Monolith patterns. I'm looking
for my first commercial role where I can apply what I've learned and continue
growing as a developer.
```

**Why it works:**
- Confident opener
- Highlights technical skills
- Shows active development
- Demonstrates learning mindset
- Clear about career goals

---

#### 2. "Tell me about your MealMind project"

**Structure your answer (2-3 minutes):**

**Part 1 - What it is:**
```
MealMind is a modular nutrition tracking platform with an AI-powered chat
assistant. Users can set nutrition goals, track their intake, and get
personalized advice through a conversational AI interface.
```

**Part 2 - Why you built it:**
```
I wanted to combine my interest in nutrition with learning modern backend
architectures and AI technologies. It was also an opportunity to work with
RAG (Retrieval-Augmented Generation) and understand how to build context-aware
AI applications.
```

**Part 3 - Technical highlights:**
```
The application uses Clean Architecture with three modules: Identity, Nutrition,
and AiChat. The AI chat module implements RAG using PostgreSQL's pgvector
extension to store 768-dimensional embeddings. When users ask questions, the
system performs semantic search to find relevant nutrition information and
provides context to the LLM. I'm using Ollama for local LLM inference to keep
costs down and learn how these systems work.
```

**Part 4 - Challenges faced:**
```
One interesting challenge was figuring out the optimal chunking strategy for
RAG documents. I researched different approaches - fixed-size chunks vs semantic
chunking - and implemented a hybrid approach that respects document structure
while maintaining overlap to preserve context.
```

**Part 5 - What you'd improve:**
```
If I had more time, I'd add a comprehensive test suite, implement a frontend
UI, and deploy it to Azure. I'd also like to add integration with food databases
like OpenFoodFacts.
```

---

#### 3. "What's the difference between your projects?"

Show progression:

**TaskManager (07-08/2024):**
- Learning project
- Basic CRUD operations
- First time implementing DDD concepts
- Simple architecture

**EduSchedu (08/2024-Present):**
- More complex domain
- Applied Clean Architecture properly
- Modular Monolith structure
- Better understanding of DDD

**MealMind (10/2024-Present):**
- Most advanced
- Added CQRS pattern
- AI/ML integration
- Vector embeddings and semantic search
- Docker containerization
- Cross-module communication

**Key message:** "Each project helped me learn and apply more advanced concepts."

---

#### 4. "What do you know about Clean Architecture?"

**Good answer:**
```
Clean Architecture is an architectural pattern that emphasizes separation of
concerns and dependency inversion. The core idea is that business logic should
be independent of external concerns like databases, UI, or frameworks.

In my projects, I organize code into layers:
- Domain layer: Business entities, value objects, domain events
- Application layer: Use cases, commands, queries (CQRS)
- Infrastructure layer: Database access, external services
- API layer: Controllers, endpoints

Dependencies always point inward - the domain doesn't know about infrastructure,
which makes the code testable and maintainable.
```

---

#### 5. "What's CQRS and why did you use it?"

**Good answer:**
```
CQRS stands for Command Query Responsibility Segregation. It separates read
operations (queries) from write operations (commands).

In MealMind, I use MediatR to implement CQRS:
- Commands: Create user profile, add nutrition target, attach document
- Queries: Get user profile, get conversation messages, search documents

Benefits:
- Clear separation of concerns
- Commands can focus on business rules and validation
- Queries can be optimized for reading without affecting write models
- Makes the codebase easier to understand and maintain

For example, AddNutritionTargetCommand handles validation, creates the entity,
and publishes domain events. Meanwhile, GetUserProfileQuery just retrieves data
without any side effects.
```

---

#### 6. "What's RAG and how does it work in your project?"

**Good answer:**
```
RAG stands for Retrieval-Augmented Generation. It's a technique to give LLMs
access to specific knowledge without retraining them.

In MealMind, here's how it works:

1. Document Processing:
   - I split nutrition documents into chunks (500-800 tokens)
   - Generate 768-dimensional embeddings for each chunk using Ollama
   - Store chunks and embeddings in PostgreSQL with pgvector

2. Query Processing:
   - User asks a question like "How much protein do I need?"
   - System generates an embedding for the question
   - Performs cosine similarity search to find relevant chunks

3. Generation:
   - Top 3-5 relevant chunks are included as context
   - LLM receives: system prompt + retrieved context + user question
   - LLM generates response based on this combined information

This allows the AI to give accurate, specific nutrition advice without hallucinating.
```

---

#### 7. "Why should we hire you for this position?"

**Good answer:**
```
I believe I'm a good fit because:

1. Technical Foundation: I have hands-on experience with the technologies you
   use - C#, .NET Core, PostgreSQL, and modern architectural patterns.

2. Learning Ability: My projects show progression - from basic CRUD apps to
   implementing advanced concepts like RAG and AI integration. I'm constantly
   learning and applying new concepts.

3. Problem-Solving: Building MealMind required solving complex problems like
   chunking strategies, vector embeddings, and cross-module communication. I
   enjoy diving deep into technical challenges.

4. Best Practices: I don't just write code that works - I focus on Clean
   Architecture, DDD, and maintainable code because I understand these patterns
   make a difference in real projects.

5. Motivation: I'm eager to learn from experienced developers and contribute
   to commercial projects. I'm looking for a team where I can grow while adding
   value.
```

---

#### 8. "What are your weaknesses?"

**Bad answer:** "I'm a perfectionist" (cliché)

**Good answer:**
```
One area I'm working on is frontend development. I'm very comfortable with
backend and APIs, but I want to strengthen my frontend skills. I understand
the basics of React and how frontend communicates with backend, but I'd like
to gain more hands-on experience building user interfaces.

Another area is production deployment and DevOps. While I use Docker for local
development, I haven't deployed a production application to the cloud yet. I'm
excited to learn about CI/CD pipelines, monitoring, and cloud services like Azure.

The positive side is I'm aware of these gaps and actively working to improve them.
```

---

#### 9. "Do you have any questions for us?"

**Always ask questions!** It shows interest and helps you evaluate the company.

**Good questions:**
```
1. "What does the onboarding process look like for junior developers?"
   (Shows you're thinking about success)

2. "What technologies and patterns does your team use? How is code review done?"
   (Shows you care about learning and code quality)

3. "What does a typical project look like? Are there opportunities to work on
   different parts of the codebase?"
   (Shows interest in diverse experience)

4. "How does the team approach learning and professional development?"
   (Shows long-term thinking)

5. "What are the biggest technical challenges the team is facing right now?"
   (Shows curiosity about real work)
```

**Avoid these questions in first interview:**
- Salary (wait until they bring it up or second interview)
- Benefits/vacation (wait until later stages)
- "What does your company do?" (research beforehand!)

---

## 8. Company Research Checklist

Before applying, research the company:

### Find Out:
- ✅ What products/services do they build?
- ✅ What technologies do they use? (Check job postings, tech blog)
- ✅ Company size and structure
- ✅ Recent news or achievements
- ✅ Company culture and values
- ✅ Location and work arrangement (office/remote/hybrid)

### Where to Research:
- Company website
- LinkedIn company page
- Glassdoor reviews (take with grain of salt)
- GitHub organization (if they have open source)
- Tech blog or engineering blog
- Local tech community (if they're active)

### Use This Information:
- In cover letter: "I noticed you work with X technology..."
- In interview: "I saw on your blog that you recently..."
- To prepare questions: "I read about your project X..."

---

## 9. Application Checklist

### Before Sending Application:

#### CV Ready:
- ✅ MealMind project added
- ✅ Skills ratings updated
- ✅ Profile section improved
- ✅ Technologies section added
- ✅ No typos or formatting issues
- ✅ Contact information correct
- ✅ LinkedIn profile link works

#### GitHub Ready:
- ✅ MealMind has professional README
- ✅ Commit history is clean
- ✅ No sensitive information committed (.env files, etc.)
- ✅ Repository is public
- ✅ GitHub profile photo and bio filled out

#### LinkedIn Ready:
- ✅ Profile matches CV
- ✅ MealMind project added
- ✅ Skills section updated
- ✅ Professional photo
- ✅ Custom URL set (linkedin.com/in/your-name)

#### Mental Preparation:
- ✅ Read company research notes
- ✅ Practiced project explanation
- ✅ Prepared questions for interviewer
- ✅ Know how to get to interview location (if in-person)
- ✅ Have backup plan for technical test/coding challenge

---

## 10. After Applying

### Timeline Expectations:
- **1-3 days:** Initial screening call (HR or technical recruiter)
- **1 week:** Technical interview (coding test or technical discussion)
- **1-2 weeks:** Final interview (team fit, manager interview)
- **1 week:** Decision

**Total time:** 3-6 weeks is normal

### If You Don't Hear Back:
- Wait 1-2 weeks
- Send polite follow-up email
- Don't take it personally - many factors affect hiring

### If You Get Rejected:
- Ask for feedback (politely)
- Learn from the experience
- Apply to more positions
- Keep improving your skills

---

## 11. Salary Expectations (Poland Context)

### Junior .NET Developer (Rzeszów area):

**Typical ranges (2024):**
- **Entry Junior:** 4,000 - 6,000 PLN gross (B2B) or 3,500 - 5,000 PLN net (UoP)
- **Mid Junior:** 6,000 - 8,000 PLN gross (B2B) or 5,000 - 6,500 PLN net (UoP)

**Your range (with 3 projects + modern stack):**
- Target: 5,500 - 7,000 PLN gross (B2B) or 4,500 - 5,500 PLN net (UoP)

**Tips:**
- Don't mention number first - let them make offer
- Research typical salaries for your area
- Consider total package (training, equipment, growth opportunities)
- First job is about learning - don't focus only on salary

---

## 12. Red Flags to Watch For

During interviews, watch for these warning signs:

### Company Red Flags:
- ❌ No clear job description or expectations
- ❌ Excessive pressure to decide quickly
- ❌ No mention of mentoring/training for junior
- ❌ All negative reviews online (some negative is normal)
- ❌ Unprofessional interview behavior

### Technical Red Flags:
- ❌ Using very outdated technologies (.NET Framework 4.5, etc.)
- ❌ No code review process
- ❌ No version control (seriously!)
- ❌ No testing culture at all
- ❌ "We'll train you on our custom framework" (might be hard to transfer skills)

### These Are GOOD Signs:
- ✅ Clear onboarding process described
- ✅ Pair programming or mentoring mentioned
- ✅ Code review process in place
- ✅ Modern tech stack
- ✅ Team seems enthusiastic and helpful
- ✅ They ask about your learning style

---

## 13. Your Competitive Advantages

### What Makes You Stand Out:

1. **Modern Tech Stack**
   - Most juniors learn .NET Framework 4.x in school
   - You're using .NET 9, latest patterns
   - This is a huge advantage!

2. **Advanced Concepts**
   - RAG, AI, vector embeddings
   - Most juniors haven't touched this
   - Shows curiosity and initiative

3. **Complete Projects**
   - Not just tutorials or school assignments
   - Full architecture, from domain to API
   - Shows you can finish things

4. **Best Practices**
   - Clean Architecture, DDD, CQRS
   - Code organization
   - Git workflow

5. **Active Learning**
   - Completed paid courses (DevMentors)
   - Learning materials organized
   - Constantly improving

6. **Problem-Solving**
   - Tackled complex problems (chunking strategies, embeddings)
   - Read documentation and papers
   - Not just following tutorials

### Use These in Interview:
When asked "Why you?" - point to these differentiators.

---

## 14. Next Steps Summary

### This Week:
1. ✅ Update CV with MealMind (1 day)
2. ✅ Write GitHub README (1 day)
3. ✅ Polish LinkedIn profile (1 hour)
4. ✅ Research the company (2 hours)
5. ✅ Apply to the position!

### Next Week:
6. Add basic tests to MealMind (3-5 days)
7. Study React basics (evenings)
8. Prepare for potential interview

### Don't Overthink:
- You're ready to apply NOW
- You'll learn more in first 3 months of job than in 6 months of self-study
- Perfect is the enemy of good
- Apply, learn from the process, improve

---

## 15. Motivational Reminder

### You Are Ready Because:

✅ You have **3 complete projects** showing progression

✅ You use **modern technologies** (.NET 9, not outdated stack)

✅ You understand **architectural patterns** (Clean Architecture, DDD, CQRS)

✅ You've tackled **complex problems** (RAG, AI, embeddings)

✅ You write **organized, maintainable code**

✅ You have a **growth mindset** (learning materials, courses)

✅ You can **explain technical decisions** (why Clean Arch? why CQRS?)

### Remember:

> "Junior" doesn't mean you know nothing. It means you're eager to learn
> and grow. You already know MORE than many juniors. Your MealMind project
> would impress most senior developers.

> Companies hire juniors for potential, not perfection. They want someone
> who can learn, adapt, and grow. You've proven you can do all three.

**You've got this! 💪**

---

## Quick Reference

### Application Checklist
- [ ] CV updated with MealMind
- [ ] Skills ratings increased
- [ ] Profile section improved
- [ ] GitHub README written
- [ ] LinkedIn updated
- [ ] Company researched
- [ ] Questions prepared
- [ ] Project explanations practiced

### Interview Day
- [ ] Arrive 10 minutes early (or join 5 min early for online)
- [ ] Professional appearance
- [ ] Laptop charged (if coding test expected)
- [ ] Portfolio/GitHub ready to show
- [ ] Questions prepared
- [ ] Positive mindset

### After Interview
- [ ] Send thank-you email (24 hours)
- [ ] Note down questions you struggled with
- [ ] Continue learning
- [ ] Apply to more positions

---

## Resources

### Polish Tech Communities:
- **Devstyle** - Discord community for Polish developers
- **DevMentors** - You're already in their course!
- **Just Join IT** - Job board with salary ranges
- **No Fluff Jobs** - Tech job board
- **Bulldogjob** - Another tech job board

### Learning Resources:
- **Microsoft Learn** - Official .NET docs
- **GitHub** - Explore open source .NET projects
- **Dev.to** - Read about others' experiences
- **Stack Overflow** - Help others (shows knowledge)

### Interview Prep:
- **LeetCode** - Practice coding challenges (Easy level)
- **HackerRank** - C# coding practice
- **Pramp** - Mock technical interviews

---

## Final Thoughts

You're in a great position for a junior role. Your MealMind project demonstrates:
- Technical skills beyond typical junior level
- Problem-solving ability
- Initiative and passion
- Modern development practices

The company would be lucky to have you. Apply with confidence, be yourself in the
interview, and remember - the first job is the hardest to get. Once you have
experience, opportunities multiply.

**Go apply! You're ready. 🚀**
