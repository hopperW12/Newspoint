import { useContext, useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import { AuthContext } from "../context/AuthContext";
import ReaderBoard from "../components/ReaderBoard";
import EditorBoard from "../components/EditorBoard";
import AdminBoard from "../components/AdminBoard";
import "../assets/styles/pages/UserPage.css";

const UserPage = () => {
  const { user, jwt } = useContext(AuthContext);

  if (!user) return <p>Musíte být přihlášeni, abyste viděli tuto stránku.</p>;

  const [articles, setArticles] = useState([]);
  const [error, setError] = useState(null);

  const [newTitle, setNewTitle] = useState("");
  const [newContent, setNewContent] = useState("");
  const { role } = user;

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

  const renderBoard = () => {
    switch (role) {
      case "Reader":
        return <ReaderBoard user={user} />;
      case "Editor":
        return <EditorBoard user={user} />;
      case "Admin":
        return <AdminBoard user={user} />;
      default:
        return <p>Neznámá role: {role}</p>;
    }
  };

  // if (loading) return <p>Načítám...</p>;
  if (error) return <p>Chyba: {error}</p>;

  return (
    <>
      <Navbar />
      <div className="user-page-container">
        <h2 className="user-page-title">Můj profil - {user.unique_name}</h2>

        {/* Pridavani clanku */}
        {(user.role == "Editor" || user.role == "Admin") && (
          <div className="user-page-form-wrap">
            <h2 className="user-page-form-title">Vytvořte článek</h2>
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
          </div>
        )}
        <h2 className="user-page-board-title">Rozhraní</h2>
        {renderBoard()}
      </div>
    </>
  );
};

export default UserPage;
