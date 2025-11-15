import { useState } from "react";

const CreateComment = ({ onSubmit }) => {
  const [text, setText] = useState("");

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!text.trim()) return;

    onSubmit(text);
    setText(""); // clear textarea
  };

  return (
    <form className="article-detail-comment-form" onSubmit={handleSubmit}>
      <textarea
        value={text}
        onChange={(e) => setText(e.target.value)}
        placeholder="Napište komentář ..."
        className="article-detail-comment-input"
      />
      <button className="article-detail-button-submit" type="submit">
        Přidat komentář
      </button>
    </form>
  );
};

export default CreateComment;
