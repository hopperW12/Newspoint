import { useParams } from "react-router-dom";
import { useEffect, useState, useContext } from "react";
import Navbar from "../components/Navbar";
import defaultArticleImg from "../assets/images/default_article_img.png";
import CreateComment from "../components/CreateComment";
import { AuthContext } from "../context/AuthContext";
import "../assets/styles/pages/ArticleDetail.css";

const ArticleDetail = () => {
  const { user } = useContext(AuthContext);
  const { id } = useParams();
  const [article, setArticle] = useState(null);
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchArticle = async () => {
      try {
        const res = await fetch(`/api/Article/${id}`);
        if (!res.ok) throw new Error("Failed to fetch article");

        const article = await res.json();
        setArticle(article.data);
        setComments(article.data.comments || []);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchArticle();
  }, [id]);

  const handleAddComment = async (content) => {
    try {
      const storedJwt = localStorage.getItem("jwt");

      // TODO: Make this work on the current backend
      const res = await fetch(`/api/admin/Comment/${article.id}/comments`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${storedJwt}`,
        },
        body: JSON.stringify({ content }),
      });

      if (!res.ok) throw new Error("Nepodařilo se přidat komentář");

      // backend vrátí nový koment
      const savedComment = await res.json();

      // přidání nového komentáře do local state, aby se okamžitě zobrazil
      setComments([savedComment, ...comments]);
    } catch (err) {
      console.error(err);
      alert("Došlo k problému při tvorbě komentáře.");
    }
  };

  if (loading) return <p>Loading article...</p>;
  if (error) return <p>Error: {error}</p>;
  if (!article) return <p>Article not found.</p>;

  return (
    <>
      <Navbar />
      <div className="article-detail">
        <h2 className="article-detail-title">{article.title}</h2>
        <div className="article-detail-author-wrap">
          <div className="article-detail-author-circle">
            {article.author
              .split(" ")
              .map((word) => word[0])
              .join("")
              .toUpperCase()}
          </div>
          <p className="article-detail-author-name">{article.author}</p>
          <p className="article-detail-release-date">
            {new Date(article.publishedAt)
              .toLocaleString("cs-CZ", {
                year: "numeric",
                month: "2-digit",
                day: "2-digit",
                hour: "2-digit",
                minute: "2-digit",
              })
              .split(". ")
              .join(".")}
          </p>
        </div>
        <p className="article-detail-category">{article.category}</p>
        <img className="article-detail-img" src={defaultArticleImg} alt="" />
        <p className="article-detail-text">{article.content}</p>
        <hr className="article-detail-divider" />

        <h2 className="article-detail-comments-title">Komentáře</h2>

        {/* NEW: only show comment form if user is logged in */}
        {user && <CreateComment onSubmit={handleAddComment} />}

        <div className="article-detail-comments-wrap">
          {comments.map((comment) => (
            <div className="article-detail-comment" key={comment.id}>
              <div className="article-detail-comment-header">
                <div className="article-detail-comment-author">
                  {comment.author}
                </div>
                <div className="article-detail-comment-date">
                  {new Date(comment.publishedAt)
                    .toLocaleString("cs-CZ", {
                      year: "numeric",
                      month: "2-digit",
                      day: "2-digit",
                      hour: "2-digit",
                      minute: "2-digit",
                    })
                    .split(". ")
                    .join(".")}
                </div>
              </div>
              <div className="article-detail-comment-content">
                {comment.content}
              </div>
            </div>
          ))}
        </div>
      </div>
    </>
  );
};

export default ArticleDetail;
