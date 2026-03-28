const Player = require('../models/Player');
const { generateToken } = require('../utils/jwt');

const bcrypt = require('bcryptjs');

exports.createPlayer = async (req, res, next) => {
    try {
        const { username, email, password } = req.body;
        
        const player = await Player.create({
            username,
            email,
            password
        });

        res.status(201).json({
            success: true,
            data: player
        })
    } catch (error) {
        console.error('MINIMAL REG ERROR:', error);
        res.status(500).json({ success: false, error: error.message });
    }
}

exports.login = async (req, res) => {
    try {
        const { username, password } = req.body;
        if (!username || !password) {
            return res.status(400).json({
                success: false,
                message: "Please provide a valid username and password"
            });
        }

        const player = await Player.findOne({ username });
        if (!player || !(await player.comparePassword(password))) {
            return res.status(401).json({
                success: false,
                message: "Invalid username or password"
            });
        }

        const token = generateToken(player._id);

        res.status(200).json({
            success: true,
            message: "Login successful",
            token: token,
            data: {
                id: player._id,
                username: player.username,
                email: player.email,
                kills: player.kills,
                deaths: player.deaths
            }
        });
    }
    catch (error) {
        res.status(500).json({
            success: false,
            message: error.message
        });
    }
}

exports.updateScore = async (req, res) => {
    try {
        const { kills, deaths } = req.body;
        const id = req.params.id;
        if (req.player.id !== id) {
            return res.status(403).json({
                success: false,
                message: 'You can only update your own stats'
            });
        }

        const player = await Player.findByIdAndUpdate(
            id,
            { $inc: { kills: kills || 0, deaths: deaths || 0 } },
            { new: true, runValidators: true }
        );

        if (!player) {
            return res.status(404).json({
                success: false,
                message: 'Player not found'
            });
        }

        res.status(200).json({
            success: true,
            data: player
        });
    }
    catch (error) {
        res.status(500).json({
            success: false,
            message: 'Failed to update Player K/D',
            error: error.message
        });
    }
}

exports.getAllPlayers = async (req, res) => {
    try {
        const players = await Player.find().select('-password');
        res.status(200).json({
            success: true,
            count: players.length,
            data: players
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            message: 'Failed to get players',
            error: error.message
        });
    }
}

exports.getPlayerById = async (req, res) => {
    try {
        const player = await Player.findById(req.params.id).select('-password');
        if (!player) {
            return res.status(404).json({
                success: false,
                message: 'Player not found'
            });
        }
        res.status(200).json({
            success: true,
            data: player
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            message: 'Server Error',
            error: error.message
        });
    }
}

exports.deletePlayer = async (req, res) => {
    try {
        const id = req.params.id;
        if (req.player.id !== id) {
            return res.status(403).json({
                success: false,
                message: 'You can only delete your own account'
            });
        }

        const player = await Player.findByIdAndDelete(id);

        if (!player) {
            return res.status(404).json({
                success: false,
                message: 'Player not found'
            });
        }

        res.status(200).json({
            success: true,
            message: 'Player deleted successfully'
        });
    } catch (error) {
        res.status(500).json({
            success: false,
            message: 'Failed to delete player',
            error: error.message
        });
    }
}