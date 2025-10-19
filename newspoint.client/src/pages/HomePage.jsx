import React from "react";
import { useState, useEffect } from "react";
import Navbar from "../components/Navbar";
import Article from "../components/Article";
import "../assets/styles/pages/HomePage.css";

const HomePage = () => {
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    fetch("/api/article")
      .then((response) => {
        if (!response.ok) {
          throw new Error("Chyba při načítání článků");
        }
        return response.json();
      })
      .then((data) => {
        setArticles(data);
        setLoading(false);
      })
      .catch((err) => {
        setError(err.message);
        setLoading(false);
      });
  }, []);

  if (loading) return <p>Načítám články...</p>;
  if (error) return <p>Chyba: {error}</p>;

  return (
    <main className="homepage-main">
      <Navbar />
      <div className="articles-wrap">
        {articles.map((article) => (
          <Article key={article.id} article={article} />
        ))}
      </div>
    </main>
  );
};

export default HomePage;
