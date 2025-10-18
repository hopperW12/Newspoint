import React from "react";

const Article = ({ article }) => {
  return (
    <div className="article">
      <h1>{article.title}</h1>
    </div>
  );
};

export default Article;
