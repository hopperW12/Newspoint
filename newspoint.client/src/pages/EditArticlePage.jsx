import { useState, useEffect, useContext } from "react";
import { useParams, useNavigate, data, Link, redirect } from "react-router-dom";
import { AuthContext } from "../context/AuthContext";
import Navbar from "../components/Navbar";
import "../assets/styles/pages/EditArticle.css";

const EditArticlePage = () => {
  const { user, jwt } = useContext(AuthContext);
  const { id } = useParams();
  const navigate = useNavigate();

  const [article, setArticle] = useState(null);
  const [title, setTitle] = useState("");
  const [content, setContent] = useState("");
  const [image, setImage] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchArticle = async () => {
      try {
        const res = await fetch(`/api/article/${id}`);
        if (!res.ok) throw new Error("Chyba při načítání článku.");
        const articleData = await res.json();

        // Povolit editaci pouze autorovi nebo adminovi

        if (
          user.nameid != articleData.data.authorId &&
          (user.role !== "Admin" || user.role !== "Editor")
        ) {
          navigate(`/Articles/${id}`);
          return;
        }

        // Předvyplnění inputů
        setArticle(articleData.data);
        setTitle(articleData.data.title);
        setContent(articleData.data.content);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };
    fetchArticle();
  }, [id, user, navigate]);

  const handleUpdate = async (e) => {
    e.preventDefault();

    try {
      const formData = new FormData();

      // Přidáme jen změněné hodnoty
      if (title !== article.title) formData.append("title", title);
      if (content !== article.content) formData.append("content", content);
      if (image) formData.append("image", image);

      // Povolit editaci pouze autorovi a zároveň Adminovi nebo Editorovi
      if (
        user.id !== articleData.authorId ||
        (user.role !== "Admin" && user.role !== "Editor")
      ) {
        alert("Nemáte oprávnění upravovat tento článek.");
        navigate(`/Articles/${id}`);
        return;
      }

      const res = await fetch(`/api/account/article/${id}`, {
        method: "PUT",
        headers: {
          Authorization: `Bearer ${jwt}`,
        },
        body: formData,
      });

      if (!res.ok) throw new Error("Chyba při aktualizaci článku.");
      navigate(`/Articles/${id}`);
    } catch (err) {
      alert(err.message);
    }
  };

  if (loading) return <p>Načítám...</p>;
  if (error) return <p>Chyba: {error}</p>;

  return (
    <>
      <Navbar />
      <div className="edit-article-container">
        <h2 className="edit-article-title">Upravit článek</h2>
        <p>
          <Link to={`/Articles/${id}`} className="edit-article-back-link">
            ← Zpět na článek
          </Link>
        </p>
        <form className="edit-article-form" onSubmit={handleUpdate}>
          <input
            type="text"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            placeholder="Nadpis článku"
            required
          />
          <textarea
            value={content}
            onChange={(e) => setContent(e.target.value)}
            placeholder="Obsah článku"
            required
          />
          <label>Nahrát nový obrázek (volitelné)</label>
          <input
            type="file"
            accept="image/*"
            onChange={(e) => setImage(e.target.files[0])}
          />
          <button type="submit">Uložit změny</button>
        </form>
      </div>
    </>
  );
};

export default EditArticlePage;
