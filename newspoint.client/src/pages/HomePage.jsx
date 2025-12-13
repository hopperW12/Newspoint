import { useState, useEffect } from "react";
import Navbar from "../components/Navbar";
import Article from "../components/Article";
import "../assets/styles/pages/HomePage.css";

const HomePage = () => {
  const [articles, setArticles] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategories, setSelectedCategories] = useState(["all"]);
  const [searchQuery, setSearchQuery] = useState("");

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Getne vsechny articles a categories
  useEffect(() => {
    const fetchData = async () => {
      try {
        const [articlesRes, categoriesRes] = await Promise.all([
          fetch("/api/article"),
          fetch("/api/category"),
        ]);

        if (!articlesRes.ok || !categoriesRes.ok)
          throw new Error("Chyba při načítání dat.");

        const articlesData = await articlesRes.json();
        const categoriesData = await categoriesRes.json();

        setArticles(articlesData);
        setCategories(categoriesData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, []);

  if (loading) return <p>Načítám...</p>;
  if (error) return <p>Chyba: {error}</p>;

  // Vyber categorii
  const toggleCategory = (id) => {
    // Klik na "all" vypne ostatni kategorie
    if (id === "all") {
      setSelectedCategories(["all"]);
      return;
    }

    // all se vzpne kdyz jsou vybrane jine kategorie
    const withoutAll = selectedCategories.filter((c) => c !== "all");

    // odkliknuti kategorie
    if (withoutAll.includes(id)) {
      const updated = withoutAll.filter((c) => c !== id);

      // jestli uz neni zadna kategorie tak se zapne all
      setSelectedCategories(updated.length === 0 ? ["all"] : updated);
    } else {
      // pridani nove kategorie
      setSelectedCategories([...withoutAll, id]);
    }
  };

  // filtrování 
  const search = searchQuery.trim().toLowerCase();

  const categoryFiltered = selectedCategories.includes("all")
    ? articles
    : articles.filter((a) =>
        selectedCategories.includes(String(a.categoryId))
      );

  const filteredArticles = categoryFiltered.filter((a) => {
    if (search === "") return true;

    const title = (a.title || "").toLowerCase();
    const content = (a.content || "").toLowerCase();
    const author = (a.author || "").toLowerCase();

    let dateText = "";
    if (a.publishedAt) {
      dateText = new Date(a.publishedAt)
        .toLocaleString("cs-CZ", {
          year: "numeric",
          month: "2-digit",
          day: "2-digit",
          hour: "2-digit",
          minute: "2-digit",
        })
        .split(". ")
        .join(".")
        .toLowerCase();
    }

    return (
      title.includes(search) ||
      content.includes(search) ||
      author.includes(search) ||
      dateText.includes(search)
    );
  });

  return (
    <main className="homepage-main">
      <Navbar />
        
      <div className="homepage-categories-wrap">
        <span
          className={`category-pill ${
            selectedCategories.includes("all") ? "active" : ""
          }`}
          onClick={() => toggleCategory("all")}
        >
          Všechny
        </span>

        {categories.map((c) => (
          <span
            key={c.id}
            className={`category-pill ${
              selectedCategories.includes(String(c.id)) ? "active" : ""
            }`}
            onClick={() => toggleCategory(String(c.id))}
          >
            {c.name}
          </span>
        ))}
      </div>
        
        <div className="homepage-search-wrap">
            <input
                type="text"
                className="homepage-search-input"
                placeholder="Hledat podle titulku, datumu, obsahu nebo autora..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
            />
        </div>
        
      <div className="articles-wrap">
        {filteredArticles.length === 0 ? (
          <p className="homepage-no-articles">Žádné články v této kategorii.</p>
        ) : (
          [...filteredArticles]
            .sort((a, b) => new Date(b.publishedAt) - new Date(a.publishedAt))
            .map((article) => <Article key={article.id} article={article} />)
        )}
      </div>
    </main>
  );
};

export default HomePage;
