const mongoose = require('mongoose');
const bcrypt = require('bcryptjs');

const playerSchema = new mongoose.Schema({
    username:{
        type: String,
        required: [true, `Username is required`],
        unique: true,
        trim: true,
        minlength: [3,`Username must be at least 3 characters`],
        maxlength: [20, `Username must not exceed 20 characters`]
    },
    email:{
        type: String,
        required: [true, `Email is required`],
        unique: true,
        lowercase: true,
        match:[/^\S+@\S+$/, `Please provide a valid email`]
    },
    password:{
        type: String,
        required: [true, `Password is required`],
        minlength: [8, `Password must be at least 8 characters`]
    },
    hp:{
        type: Number,
        default:100,
        min: [0,'HP cannot be below 0'],
        max: [100,'HP cannot exceed 100'],
    },
    kills:{
        type: Number,
        default:0,
        min: [0,'Kills cannot be below 0'],
    },
    deaths:{
        type: Number,
        default:0,
        min: [0,'Deaths cannot be below 0'],
    }
},{
    timestamps: true
});

playerSchema.methods.comparePassword = async function(enteredPassword){
    return await bcrypt.compare(enteredPassword, this.password)
};

const Player = mongoose.model('Player', playerSchema);
module.exports = Player;