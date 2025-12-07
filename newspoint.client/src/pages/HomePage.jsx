import { useState, useEffect } from "react";
import Navbar from "../components/Navbar";
import Article from "../components/Article";
import "../assets/styles/pages/HomePage.css";

const HomePage = () => {
  const [articles, setArticles] = useState([]);
  const [categories, setCategories] = useState([]);
  const [selectedCategories, setSelectedCategories] = useState(["all"]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

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

  // Filtrovani clanku
  const filteredArticles = selectedCategories.includes("all")
    ? articles
    : articles.filter((a) => selectedCategories.includes(String(a.categoryId)));

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
