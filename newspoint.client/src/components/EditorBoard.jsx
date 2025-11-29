import { useState, useEffect, useContext } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

const EditorBoard = () => {
  const { user, jwt } = useContext(AuthContext);
  const [articles, setArticles] = useState([]);
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [newTitle, setNewTitle] = useState("");
  const [newContent, setNewContent] = useState("");

  useEffect(() => {
    if (!user) return;

    const fetchArticles = async () => {
      try {
        const res = await fetch(`/api/account/articles`, {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!res.ok) throw new Error("Chyba při načítání článků");
        const data = await res.json();
        setArticles(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    const fetchComments = async () => {
      try {
        const res = await fetch(`/api/account/comments`, {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!res.ok) throw new Error("Chyba při načítání komentářů");
        const data = await res.json();
        setComments(data);
      } catch (err) {
        console.error(err.message);
      }
    };

    fetchArticles();
    fetchComments();
  }, [user, jwt]);

  const handleDeleteArticle = async (id) => {
    if (!window.confirm("Opravdu chcete smazat tento článek?")) return;
    try {
      const res = await fetch(`/api/account/article/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${jwt}` },
      });
      if (!res.ok) throw new Error("Chyba při mazání článku");
      setArticles(articles.filter((a) => a.id !== id));
    } catch (err) {
      alert(err.message);
    }
  };

  if (!user) return <p>Musíte být přihlášeni, abyste viděli tuto stránku.</p>;
  if (loading) return <p>Načítám...</p>;
  if (error) return <p>Chyba: {error}</p>;

  return (
    <div className="editor-board">
      <div className="reader-section">
        <h4>Uživatelské informace</h4>
        <p>
          <strong>Jméno:</strong> {user.unique_name}
        </p>
        <p>
          <strong>Email:</strong> {user.email}
        </p>
        <p>
          <strong>Role:</strong> {user.role}
        </p>
      </div>

      <div className="editor-section">
        <h4 className="editor-sections-title">Moje články</h4>
        {articles.length === 0 && (
          <p className="editor-noarticles-text">
            Ještě nemáte žádné články. 📝
          </p>
        )}
        <ul className="editor-articles-list">
          {articles.map((a) => (
            <li key={a.id} className="editor-article-item">
              <div className="editor-article-header">
                <Link to={`/Articles/${a.id}`} className="editor-article-link">
                  <strong className="editor-article-item-name">
                    {a.title}
                  </strong>
                </Link>
                <button
                  className="editor-article-delete-btn"
                  onClick={() => handleDeleteArticle(a.id)}
                >
                  Smazat
                </button>
              </div>
              <p className="editor-article-date">
                {new Date(a.publishedAt).toLocaleDateString("cs-CZ", {
                  year: "numeric",
                  month: "long",
                  day: "numeric",
                })}
              </p>
            </li>
          ))}
        </ul>
      </div>

      <div className="editor-section">
        <h4 className="editor-sections-title">Moje komentáře</h4>
        {comments.length === 0 && <p>Ještě nejsou žádné komentáře. 📝</p>}
        <div className="article-detail-comments-wrap">
          {comments.map((c) => (
            <div className="article-detail-comment" key={c.id}>
              <div className="article-detail-comment-header">
                <div className="article-detail-comment-author">
                  <p>{c.author}</p>
                </div>
                <div className="article-detail-comment-date">
                  <p>
                    {new Date(c.publishedAt)
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
              </div>
              <div className="article-detail-comment-content">
                <p>{c.content}</p>
              </div>
              <Link
                to={`/articles/${c.articleId}`}
                className="reader-board-article-link"
              >
                Přejít na článek
              </Link>
            </div>
          ))}
        </div>
      </div>
    </div>
  );
};

export default EditorBoard;
