import { useState, useEffect, useContext } from "react";
import { Link } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";

const AdminBoard = () => {
  const { user, jwt } = useContext(AuthContext);
  const [articles, setArticles] = useState([]);
  const [myArticles, setMyArticles] = useState([]);
  const [comments, setComments] = useState([]);
  const [users, setUsers] = useState([]);

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    if (!user) return;

    const fetchArticles = async () => {
      try {
        const res = await fetch(`/api/Article`, {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!res.ok) throw new Error("Chyba při načítání článků");
        const data = await res.json();

        setArticles(data);
        setMyArticles(data.filter((a) => a.authorId == user.nameid));
      } catch (err) {
        setError(err.message);
      }
    };

    const fetchComments = async () => {
      try {
        const res = await fetch(`/api/account/comment`, {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!res.ok) throw new Error("Chyba při načítání komentářů");
        const data = await res.json();
        setComments(data);
      } catch (err) {
        console.error(err.message);
      }
    };

    const fetchUsers = async () => {
      try {
        const res = await fetch(`/api/admin/user`, {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!res.ok) throw new Error("Chyba při načítání uživatelů");
        const data = await res.json();
        setUsers(data);
      } catch (err) {
        console.error(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchArticles();
    fetchComments();
    fetchUsers();
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

  const handleToggleRole = async (userId, currentRole) => {
    try {
      const newRole = currentRole === "Reader" ? "Editor" : "Reader";

      const res = await fetch(`/api/admin/user/${userId}/role`, {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${jwt}`,
        },
        body: JSON.stringify({ role: newRole }),
      });

      if (!res.ok) throw new Error("Chyba při změně role uživatele");

      setUsers(
        users.map((u) =>
          u.id === userId
            ? { ...u, roleName: newRole, role: newRole === "Editor" ? 2 : 1 }
            : u
        )
      );
    } catch (err) {
      alert(err.message);
    }
  };

  const handleDeleteUser = async (id) => {
    if (!window.confirm("Opravdu chcete smazat tohoto uživatele?")) return;

    try {
      const res = await fetch(`/api/admin/user/${id}`, {
        method: "DELETE",
        headers: { Authorization: `Bearer ${jwt}` },
      });
      if (!res.ok) throw new Error("Chyba při mazání uživatele");

      setUsers(users.filter((u) => u.id !== id));
    } catch (err) {
      alert(err.message);
    }
  };

  if (!user) return <p>Musíte být přihlášeni.</p>;
  if (loading) return <p>Načítám…</p>;
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

      {/* ==== Admin sekce - správa uživatelů ==== */}
      <div className="admin-user-management-section">
        <h4 className="admin-user-management-title">Správa uživatelů</h4>

        {users.length === 0 && (
          <p className="admin-user-management-empty">Žádní uživatelé.</p>
        )}

        <ul className="admin-user-management-list">
          {users.map((u) => (
            <li key={u.id} className="admin-user-management-item">
              <div className="admin-user-info">
                <span className="admin-user-name">
                  {u.firstName} {u.lastName}
                </span>
                <span className="admin-user-email">{u.email}</span>
                <span className="admin-user-role">{u.roleName}</span>
              </div>

              <div className="admin-user-actions">
                {u.roleName !== "Admin" && (
                  <>
                    <button
                      className="admin-user-delete-btn"
                      onClick={() => handleDeleteUser(u.id)}
                    >
                      Smazat
                    </button>
                    <button
                      className="admin-user-toggle-role-btn"
                      onClick={() => handleToggleRole(u.id, u.roleName)}
                    >
                      {u.roleName === "Reader"
                        ? "Změnit na Editor"
                        : "Změnit na Reader"}
                    </button>
                  </>
                )}
              </div>
            </li>
          ))}
        </ul>
      </div>

      {/* Moje články */}
      <div className="editor-section">
        <h4 className="editor-sections-title">Moje články</h4>

        {myArticles.length === 0 && (
          <p className="editor-noarticles-text">Nemáte žádné články. 📝</p>
        )}

        <ul className="editor-articles-list">
          {myArticles.map((a) => (
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

      {/* Všechny články */}
      <div className="editor-section">
        <h4 className="editor-sections-title">Všechny články</h4>

        {articles.length === 0 && (
          <p className="editor-noarticles-text">Nic tu zatím není. 📝</p>
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
              <p className="editor-article-author">{a.author}</p>
            </li>
          ))}
        </ul>
      </div>

      {/* Komentáře */}
      <div className="editor-section">
        <h4 className="editor-sections-title">Moje komentáře</h4>

        {comments.length === 0 && <p>Žádné komentáře.</p>}

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

export default AdminBoard;
