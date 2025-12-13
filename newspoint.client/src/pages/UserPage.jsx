import { useContext, useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import { AuthContext } from "../context/AuthContext";
import ReaderBoard from "../components/ReaderBoard";
import EditorBoard from "../components/EditorBoard";
import AdminBoard from "../components/AdminBoard";
import "../assets/styles/pages/UserPage.css";

const UserPage = () => {
  const { user, jwt } = useContext(AuthContext);

  if (!user) {
    return <p>Musíte být přihlášeni, abyste viděli tuto stránku.</p>;
  }

  const [articles, setArticles] = useState([]);
  const [categories, setCategories] = useState([]);
  const [error, setError] = useState(null);

  const [newTitle, setNewTitle] = useState("");
  const [newContent, setNewContent] = useState("");
  const [newImage, setNewImage] = useState(null);
  const [newCategory, setNewCategory] = useState("");

  const [successMessage, setSuccessMessage] = useState("");

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const res = await fetch("/api/category");
        if (!res.ok) throw new Error("Nepodařilo se načíst kategorie.");

        const data = await res.json();
        setCategories(data);
      } catch (err) {
        console.error(err);
      }
    };

    fetchCategories();
  }, []);

  const handleCreate = async (e) => {
    e.preventDefault();

    try {
      const formData = new FormData();
      formData.append("title", newTitle);
      formData.append("content", newContent);
      formData.append("categoryId", newCategory);

      if (newImage) formData.append("image", newImage);

      const res = await fetch("/api/account/article", {
        method: "POST",
        headers: {
          Authorization: `Bearer ${jwt}`,
        },
        body: formData,
      });

      if (!res.ok) throw new Error("Chyba při vytváření článku");

      // ✔ úspěšně vytvořeno
      setSuccessMessage("Článek byl úspěšně vytvořen 🎉");

      // reset formuláře
      setNewTitle("");
      setNewContent("");
      setNewImage(null);
      setNewCategory("");

      // reload stránky po krátké době
      setTimeout(() => {
        window.location.reload();
      }, 1500);
    } catch (err) {
      alert(err.message);
    }
  };

  const renderBoard = () => {
    switch (user.role) {
      case "Reader":
        return <ReaderBoard user={user} />;
      case "Editor":
        return <EditorBoard user={user} />;
      case "Admin":
        return <AdminBoard user={user} />;
      default:
        return <p>Neznámá role: {user.role}</p>;
    }
  };

  return (
    <>
      <Navbar />

      <div className="user-page-container">
        <h2 className="user-page-title">Můj profil – {user.unique_name}</h2>

        {(user.role === "Editor" || user.role === "Admin") && (
          <div className="admin-create-article-box">
            <h2 className="admin-create-article-title">Vytvořit nový článek</h2>

            <form className="admin-create-article-form" onSubmit={handleCreate}>
              <input
                type="text"
                placeholder="Nadpis článku"
                className="admin-create-article-input"
                value={newTitle}
                onChange={(e) => setNewTitle(e.target.value)}
                required
              />

              <textarea
                placeholder="Obsah článku"
                className="admin-create-article-textarea"
                value={newContent}
                onChange={(e) => setNewContent(e.target.value)}
                required
              />

              <label className="admin-create-article-label">Kategorie</label>
              <select
                className="admin-create-article-select"
                value={newCategory}
                onChange={(e) => setNewCategory(e.target.value)}
                required
              >
                <option value="">Vyberte kategorii...</option>
                {categories.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.name}
                  </option>
                ))}
              </select>

              <label className="admin-create-article-label">
                Obrázek (volitelně)
              </label>
              <input
                type="file"
                accept="image/*"
                className="admin-create-article-input"
                onChange={(e) => setNewImage(e.target.files[0])}
              />

              {successMessage && (
                <div className="admin-create-article-success">
                  {successMessage}
                </div>
              )}

              <button className="admin-create-article-btn" type="submit">
                Vytvořit článek
              </button>
            </form>
          </div>
        )}

        {error && <p className="user-page-error">{error}</p>}

        {renderBoard()}
      </div>
    </>
  );
};

export default UserPage;
