import { Link } from "react-router-dom";
import defaultArticleImg from "../assets/images/default_article_img.png";

const Article = ({ article }) => {
  return (
    <article className="article-card" data-id="4">
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
