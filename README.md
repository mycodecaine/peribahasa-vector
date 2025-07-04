# Malay Peribahasa Vector Search

A modern AI-powered system to store and search **Malay proverbs (peribahasa)** by **text** or **meaning** using **vector embeddings**.

## 📖 Overview

This project builds a searchable database of Malay peribahasa where users can:
- Search using the exact proverb
- Search by describing the **meaning or moral** of the proverb
- Retrieve the most relevant proverbs using **semantic similarity**

This project combines **linguistic heritage** with **AI/ML technologies** to help preserve, learn, and explore Malay culture more meaningfully.

---

## 🔍 Example Use Cases

| Query Type                | Example Input                                   | Result                                           |
|--------------------------|--------------------------------------------------|--------------------------------------------------|
| By Peribahasa            | `Bagai aur dengan tebing`                       | Returns the matching proverb and its meaning     |
| By Meaning (in Malay)    | `kerjasama atau saling membantu`                | Suggests related proverbs                        |
| By Meaning (in English)  | `mutual help or cooperation`                    | Suggests `Bagai aur dengan tebing`, etc.         |

---

## 💡 Project Goals

- Create a **vector-based searchable database** for Malay peribahasa
- Enable **semantic search** using natural language queries
- Promote Malay wisdom and proverbs through accessible modern tools

---

## 🛠 Tech Stack

| Area              | Tools/Tech                                              |
|-------------------|---------------------------------------------------------|
| Embedding Model   | OpenAI (text-embedding-ada), Ollama (nomic-embed-text)  |
| Backend           | .NET (ASP.NET Core)                                     |
| Vector DB         | pgvector                                                |
| Data Source       | Curated Malay proverb dataset (CSV / JSON format)       |
| Client (Optional) | Web UI with search box or ChatGPT plugin integration    |

