import axios from "axios";
export async function loginUsers(data) {
    const token= await axios.post(`http://localhost:8000/api/token/`,data,{
        headers:{
            "content-type":"application/json",
        },
    });
  
    
    return token;

}