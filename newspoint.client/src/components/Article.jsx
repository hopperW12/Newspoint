import React from "react";

const Article = ({ article }) => {
  return (
    <article className="article-card" data-id="4">
      <header className="article-header">
        <div className="article-title-wrap">
          <h1 className="article-title">{article.title}</h1>
          <div className="article-category">{article.category}</div>
        </div>

        <div className="article-meta">
          <p className="article-author">
            <span className="article-avatar" aria-hidden="true">
              {article.author
                .split(" ")
                .map((n) => n[0])
                .join("")
                .toUpperCase()}
            </span>
            {article.author}
          </p>

          <time className="article-published">
            {new Date(article.publishedAt).toLocaleDateString("cs-CZ", {
              year: "numeric",
              month: "long",
              day: "numeric",
            })}
          </time>
        </div>
      </header>

      <section className="article-content">
        <p>{article.content}</p>
      </section>

      <section className="articles-comments">
        <h2 className="articles-comments-title">
          Comments <small>(0)</small>
        </h2>
        <div className="articles-comments-empty">Zatím žádné komentáře ...</div>
      </section>
    </article>
  );
};

export default Article;
