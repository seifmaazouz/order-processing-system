import axios from "axios";
export async function loginUsers(data) {
    const token= await axios.post(`http://localhost:8080/api/auth/login`,data,{
        headers:{
            "Content-Type":"application/json",
        },
    });
  
    
    return token;

}