export async function registerUser(data) {
    const token= await axios.post(`http://localhost:8000/api/register/`,data,{
        headers:{
            "content-type":"application/json",
        },
    });
  
    
    return token;

}