import { useParams } from "react-router-dom";
import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import "../assets/styles/pages/ArticleDetail.css";
import defaultArticleImg from "../assets/images/default_article_img.png";
import CreateComment from "../components/CreateComment";

const ArticleDetail = () => {
  const { id } = useParams();
  const [article, setArticle] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  console.log(article);

  useEffect(() => {
    const fetchArticle = async () => {
      try {
        const res = await fetch(`/api/Article/${id}`);

        if (!res.ok) {
          throw new Error("Failed to fetch article");
        }

        const article = await res.json();
        setArticle(article.data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchArticle();
  }, [id]);

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
        <CreateComment />
        <div className="article-detail-comments-wrap">
          {article.comments.map((comment, index) => (
            <div className="article-detail-comment" key={comment.id || index}>
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
