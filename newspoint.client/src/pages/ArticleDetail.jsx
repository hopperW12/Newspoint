import { useParams } from "react-router-dom";
import { useEffect, useState, useContext } from "react";
import Navbar from "../components/Navbar";
import defaultArticleImg from "../assets/images/default_article_img.png";
import CreateComment from "../components/CreateComment";
import { AuthContext } from "../context/AuthContext";
import { TiDelete } from "react-icons/ti";
import "../assets/styles/pages/ArticleDetail.css";

const ArticleDetail = () => {
  const { user } = useContext(AuthContext);
  const { id } = useParams();
  const [article, setArticle] = useState(null);
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Nacteni detailu clanku
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

  // Pridavani komentaru
  const handleAddComment = async (content) => {
    try {
      const storedJwt = localStorage.getItem("jwt");

      const res = await fetch("/api/account/comment", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${storedJwt}`,
        },
        body: JSON.stringify({
          Content: content,
          ArticleId: id,
        }),
      });

      if (!res.ok) throw new Error("Nepodařilo se přidat komentář");

      // backend vrátí nový koment
      const savedComment = await res.json();

      // přidání nového komentáře do local state, aby se okamžitě zobrazil
      setComments([savedComment.data, ...comments]);
    } catch (err) {
      console.error(err);
      alert("Došlo k problému při tvorbě komentáře.");
    }
  };

  const handleDeleteComment = async (commentId) => {
    try {
      const storedJwt = localStorage.getItem("jwt");

      const res = await fetch(`/api/account/comment/${commentId}`, {
        method: "DELETE",
        headers: {
          Authorization: `Bearer ${storedJwt}`,
        },
      });

      if (!res.ok) throw new Error("Komentář se nepodařilo smazat");

      // Odstranit komentář z frontendu
      setComments((prev) => prev.filter((c) => c.id !== commentId));
    } catch (err) {
      console.error(err);
      alert(err.message);
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

        {/* ukaz comment form jen pokud je uzivatel prihlaseny */}
        {user && <CreateComment onSubmit={handleAddComment} />}

        <div className="article-detail-comments-wrap">
          {comments.map((comment) => (
            <div className="article-detail-comment" key={comment.id}>
              <div className="article-detail-comment-header">
                <div className="article-detail-comment-author">
                  <p>{comment.author}</p>
                </div>
                <div className="article-detail-comment-date">
                  <p>
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
                  </p>
                  {comment.authorId == +user.nameid && (
                    <button
                      onClick={() => handleDeleteComment(comment.id)}
                      className="article-detail-comment-delete-btn"
                    >
                      <TiDelete />
                    </button>
                  )}
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
