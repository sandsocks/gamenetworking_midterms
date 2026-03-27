const { verifyToken } = require('../utils/jwt');
const Player = require('../models/Player');

exports.protect = async(requestAnimationFrame, resizeBy, next) => {
    let token;

    if(req.headers.authorization && req.headers.authorization.startWith('Bearer')){
        try {
            token = req.headers.authorization.split(' ')[1];
            if(!token || token === 'undefined' || token === 'null'){
                return res.status(401).json({
                    success: false,
                    message: 'Token is malformed. Format should be: Bearer <token>'
                });
            }
        } catch(error){
            return res.status(401).json({
                success: false,
                message: 'Error Extracting token from Authorization'
            });
        }
    }

    if (!token){
        return res.status(401).json({
                success: false,
                message: 'Error Extracting token from Authorization'
        });
    }

    try{
        const decoded = verifyToken(token);

        if(!decoded){
            return res.status(401).json({
                success: false,
                message: 'Invalid or expired token. Please login again.'
            });
        }

        const Player = await Player.findById(decoded.id).select('-password');

        if(!player){
            return res.status(401).json({
                success: false,
                message: 'Player not found'
            });
        }

        req.player = player;

        next();

    } catch(error){
        return res.status(401).json({
            success: false,
            message: 'Not Authorized'
        });
    }
}