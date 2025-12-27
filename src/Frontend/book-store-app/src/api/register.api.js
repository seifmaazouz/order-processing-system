import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

export async function registerUser(data) {
    const token= await axios.post(`${API_BASE_URL}/auth/register`,data,{
        headers:{
            "Content-Type":"application/json",
        },
    });
  
    
    return token.data;

}