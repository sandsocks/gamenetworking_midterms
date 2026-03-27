const express = require('express');
const mongoose = require('mongoose');
const dotenv = require('dotenv');

dotenv.config();

const {
    createPlayer,
    login,
    updateScore,
    getAllPlayers,
    getPlayerById,
    deletePlayer
} = require('./controllers/playerController');

const { protect } = require('./middleware/auth');

const app = express();

app.use(express.json());

// Remove global error handler temporarily
/*
app.use((err, req, res, next) => {
    console.error('--- GLOBAL ERROR ---');
    console.error(err);
    res.status(500).json({
        success: false,
        message: 'Internal Server Error',
        error: err.message
    });
});
*/

mongoose.connect(process.env.MONGODB_URI)
    .then(() => console.log('Connected to MongoDB'))
    .catch((err) => console.error('Failed to Connect to MongoDB:', err));

const PORT = process.env.PORT || 3000;

// Public routes
app.post('/players/register', createPlayer);
app.post('/players/login', login);

// Welcome route
app.get('/', (req, res) => {
    res.json({ message: 'Welcome to the Game API' });
});

// Protected routes
app.get('/players', protect, getAllPlayers);
app.get('/players/:id', protect, getPlayerById);
app.put('/players/:id', protect, updateScore);
app.delete('/players/:id', protect, deletePlayer);

app.listen(PORT, () => {
    console.log(`Server is running on port ${PORT}`);
});

