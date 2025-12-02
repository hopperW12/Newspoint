import { useContext, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

const ReaderBoard = ({ user }) => {
  const { jwt } = useContext(AuthContext);
  const [comments, setComments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!jwt) return;

    const fetchComments = async () => {
      try {
        const res = await fetch("/api/account/comment", {
          headers: {
            Authorization: `Bearer ${jwt}`,
          },
        });

        if (!res.ok) throw new Error("Chyba při načítání komentářů");

        const data = await res.json();
        setComments(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchComments();
  }, [jwt]);

  return (
    <div className="reader-board">
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

      <div className="reader-section">
        <h4>Moje komentáře</h4>

        {loading && <p>Načítám komentáře...</p>}
        {error && <p>Chyba: {error}</p>}

        {!loading && !error && comments.length === 0 && (
          <p className="placeholder">Zatím nejsou žádné komentáře 📝</p>
        )}

        {!loading && !error && comments.length > 0 && (
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
                  </div>
                </div>
                <div className="article-detail-comment-content">
                  {comment.content}
                </div>
                <Link
                  to={`/articles/${comment.articleId}`}
                  className="reader-board-article-link"
                >
                  Přejít na článek
                </Link>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
};

export default ReaderBoard;
