import axios from "axios";

export async function registerUser(data) {
    const token= await axios.post(`http://localhost:8080/api/auth/register`,data,{
        headers:{
            "Content-Type":"application/json",
        },
    });
  
    
    return token.data;

}