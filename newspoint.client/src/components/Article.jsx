import { Link } from "react-router-dom";
import defaultArticleImg from "../assets/images/default_article_img.png";

const Article = ({ article }) => {
  const formattedPublishedAt = article.publishedAt
    ? new Date(article.publishedAt)
        .toLocaleString("cs-CZ", {
          year: "numeric",
          month: "2-digit",
          day: "2-digit",
          hour: "2-digit",
          minute: "2-digit",
        })
        .split(". ")
        .join(".")
    : null;

  return (
    <article className="article-card" data-id={article.id}>
      <div className="article-title-wrap">
        <div className="article-title-inner-wrap">
          <Link to={`/Articles/${article.id}`}>
            <img
              src={article.imagePath || defaultArticleImg}
              alt="Clanek Image"
              className="article-title-img"
            />
          </Link>
        </div>
        <div className="article-title-inner-wrap">
          <Link to={`/Articles/${article.id}`} className="article-title">
            {article.title}
          </Link>
          <div className="article-category">{article.category}</div>

            <p className="article-meta">
                <span className="article-meta-author">{article.author}</span>
                <span> • </span>
                <span className="article-meta-date">{formattedPublishedAt}</span>
            </p>

          <p className="article-text">
            {article.content.split(" ").slice(0, 20).join(" ") +
              (article.content.split(" ").length > 20 ? "... " : "")}
            <Link
              className="article-detail-link"
              to={`/Articles/${article.id}`}
            >
              {" "}
              Celý článek
            </Link>
          </p>
        </div>
      </div>
    </article>
  );
};

export default Article;
