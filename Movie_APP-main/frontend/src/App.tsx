import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import Login from './components/Login';
import Register from './components/Register';
import Dashboard from "./components/Dashboard";
import Movies from './components/Movies';   
import Events from './components/Events';
import Feed from './components/Feed';
import People from "./components/People";
import Recommendation from './components/Recommendation';
import MovieDetails from './components/MovieDetails';
import WatchedMovies from "./components/WatchedMovies";
import Feedback from './components/Feedback';

function App() {
    return (
        <Router>
            <Routes>
                <Route path="/" element={<Login />} />
                <Route path="/login" element={<Login />} />
                <Route path="/register" element={<Register />} />
                <Route path="/dashboard" element={<Dashboard />} />
                <Route path="/movies" element={<Movies />} />
                <Route path="/events" element={<Events />} />
                <Route path="/feed" element={<Feed />} />
                <Route path="/people" element={<People />} />
                <Route path="/recommendation" element={<Recommendation />} />
                <Route path="/movies/:id" element={<MovieDetails />} />
                <Route path="/watched" element={<WatchedMovies />} />
                <Route path="/feedback" element={<Feedback />} />
            </Routes>
        </Router>
    );
}

export default App;
