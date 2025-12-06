import { useState, useEffect, useContext } from "react";
import { useParams, useNavigate, Link } from "react-router-dom";
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
  const [deleteImage, setDeleteImage] = useState(false);
  const [categoryId, setCategoryId] = useState(null);
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchArticleAndCategories = async () => {
      try {
        // 1. Získání článku
        const resArticle = await fetch(`/api/article/${id}`, {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!resArticle.ok) throw new Error("Chyba při načítání článku.");
        const articleData = await resArticle.json();

        // Kontrola oprávnění
        if (
          user.nameid !== articleData.data.authorId.toString() &&
          user.role !== "Admin" &&
          user.role !== "Editor"
        ) {
          navigate(`/Articles/${id}`);
          return;
        }

        setArticle(articleData.data);
        setTitle(articleData.data.title);
        setContent(articleData.data.content);
        setCategoryId(articleData.data.categoryId);
        setDeleteImage(false);

        // 2. Získání kategorií
        const resCategories = await fetch("/api/category", {
          headers: { Authorization: `Bearer ${jwt}` },
        });
        if (!resCategories.ok) throw new Error("Chyba při načítání kategorií.");
        const categoriesData = await resCategories.json();
        setCategories(categoriesData);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchArticleAndCategories();
  }, [id, user, jwt, navigate]);

  const handleUpdate = async (e) => {
    e.preventDefault();
    try {
      if (!article) return;

      const formData = new FormData();
      formData.append("Id", article.id);
      formData.append("Title", title);
      formData.append("Content", content);
      formData.append("CategoryId", categoryId);
      formData.append("AuthorId", article.authorId);
      if (image) formData.append("image", image);
      if (deleteImage) formData.append("deleteImage", true);
      formData.append("ImagePath", article.imagePath || "");

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

          <label>Vyberte kategorii</label>
          <select
            value={categoryId || ""}
            onChange={(e) => setCategoryId(Number(e.target.value))}
            required
          >
            <option value="" disabled>
              -- Vyberte kategorii --
            </option>
            {categories.map((cat) => (
              <option key={cat.id} value={cat.id}>
                {cat.name}
              </option>
            ))}
          </select>

          <div className="edit-article-delete-image-wrap">
            <label>
              <input
                type="checkbox"
                checked={deleteImage}
                onChange={(e) => setDeleteImage(e.target.checked)}
              />{" "}
              Smazat aktuální obrázek
            </label>
          </div>

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
