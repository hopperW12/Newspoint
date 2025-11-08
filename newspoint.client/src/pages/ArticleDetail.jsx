import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";

const ArticleDetail = () => {
  const { id } = useParams();
  const [article, setArticle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchArticle = async () => {
      try {
        const res = await fetch(`/api/Article/${id}`);
        if (!res.ok) throw new Error("Failed to fetch article");
        const article = await res.json();
        setArticle(article.data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchArticle();
  }, [id]);

  if (loading) return <p>Loading article...</p>;
  if (error) return <p>Error: {error}</p>;
  if (!article) return <p>Article not found.</p>;

  return (
    <>
      <Navbar />
      <div className="article-detail">
        <h2>{article.title}</h2>
        <p>
          <strong>Author:</strong> {article.author}
        </p>
        <p>
          <strong>Category:</strong> {article.category}
        </p>
        <p>{article.content}</p>
      </div>
    </>
  );
};

export default ArticleDetail;
