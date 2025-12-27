import axios from "axios";
import API_BASE_URL from '../config/api.config.js';

export async function loginUsers(data) {
    const token= await axios.post(`${API_BASE_URL}/auth/login`,data,{
        headers:{
            "Content-Type":"application/json",
        },
    });
  
    
    return token;

}