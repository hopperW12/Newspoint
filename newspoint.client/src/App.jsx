import { useEffect, useState } from 'react';
import './App.css';

function App() {
    const [articles, setArticles] = useState();

    useEffect(() => {
        getArticles();
    }, []);

    const contents = articles === undefined
        ? <p><em>Loading... Please refresh once the ASP.NET backend has started. See <a href="https://aka.ms/jspsintegrationreact">https://aka.ms/jspsintegrationreact</a> for more details.</em></p>
        : <table className="table table-striped" aria-labelledby="tableLabel">
            <thead>
                <tr>
                    <th>Title</th>
                    <th>Category</th>
                    <th>Author</th>
                    <th>Author</th>
                </tr>
            </thead>
            <tbody>
                {articles.map(article =>
                    <tr>
                        <td>{article.title}</td>
                        <td>{article.category}</td>
                        <td>{article.author}</td>
                        <td>{article.content}</td>
                    </tr>
                )}
            </tbody>
        </table>;

    return (
        <div>
            <h1 id="tableLabel">Articles</h1>
            <p>This component demonstrates fetching data from the server.</p>
            {contents}
        </div>
    );
    
    async function getArticles() {
        const response = await fetch('api/article');
        if (response.ok) {
            const data = await response.json();
            setArticles(data);
        }
    }
}

export default App;