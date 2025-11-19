import { useContext, useEffect, useState } from "react";
import { AuthContext } from "../context/AuthContext";
import "../assets/styles/pages/UserPage.css";
import Navbar from "../components/Navbar";

const UserPage = () => {
  const { user, jwt } = useContext(AuthContext);
  const [articles, setArticles] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const [newTitle, setNewTitle] = useState("");
  const [newContent, setNewContent] = useState("");

  //   useEffect(() => {
  //     const fetchArticles = async () => {
  //       try {
  //         const res = await fetch(`/api/users/${user.id}/articles`, {
  //           headers: { Authorization: `Bearer ${jwt}` },
  //         });
  //         if (!res.ok) throw new Error("Chyba při načítání článků");
  //         const data = await res.json();
  //         setArticles(data);
  //       } catch (err) {
  //         setError(err.message);
  //       } finally {
  //         setLoading(false);
  //       }
  //     };

  //     fetchArticles();
  //   }, [user, jwt]);

  const handleCreate = async (e) => {
    e.preventDefault();
    try {
      const res = await fetch(`/api/users/${user.id}/articles`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${jwt}`,
        },
        body: JSON.stringify({ title: newTitle, content: newContent }),
      });
      if (!res.ok) throw new Error("Chyba při vytváření článku");
      const created = await res.json();
      setArticles([...articles, created]);
      setNewTitle("");
      setNewContent("");
    } catch (err) {
      alert(err.message);
    }
  };

  if (!user) return <p>Musíte být přihlášeni, abyste viděli tuto stránku.</p>;
  //   if (loading) return <p>Načítám...</p>;
  if (error) return <p>Chyba: {error}</p>;

  return (
    <>
      <Navbar />
      <div className="user-page-container">
        <h2 className="user-page-title">
          {user.unique_name || user.email} – Moje články
        </h2>

        <form className="user-page-form" onSubmit={handleCreate}>
          <input
            type="text"
            placeholder="Nadpis článku"
            value={newTitle}
            className="user-page-form-input"
            onChange={(e) => setNewTitle(e.target.value)}
            required
          />
          <textarea
            placeholder="Obsah článku"
            value={newContent}
            className="user-page-form-input"
            onChange={(e) => setNewContent(e.target.value)}
            required
          />
          <div className="user-page-form-btn-wrap">
            <button className="user-page-form-btn" type="submit">
              Vytvořit nový článek
            </button>
          </div>
        </form>

        {/* <ul className="article-list">
          {articles.map((a) => (
            <li key={a.id} className="article-item">
              <h3>{a.title}</h3>
              <p>{a.content}</p>
            </li>
          ))}
        </ul> */}
      </div>
    </>
  );
};

export default UserPage;
