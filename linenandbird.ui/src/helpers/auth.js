import firebase from 'firebase/compat/app';
import axios from 'axios';

// create something that modifies a request as it goes out, 
// adding a header to it with the token
axios.interceptors.request.use((request) => {
    const token = sessionStorage.getItem('token');
    
    if (token != null) {
        request.headers.Authorization = `Bearer ${token}`;
    }
  
    return request;
  }, (err) => {
    return Promise.reject(err);
  });

const signInUser = () => {
  const provider = new firebase.auth.GoogleAuthProvider();

  //sign in and then check for a new user to post to our api
  firebase.auth().signInWithPopup(provider).then((user) => {
    if (user.additionalUserInfo?.isNewUser){
      const userInfo = {
        display_Name: user.user?.displayName,
        image_Url: user.user?.photoURL,
        firebase_Uid: user.user?.uid,
        email: user.user?.email,
      }

      //add the user to your api and database

      window.location.href = '/';
    }
  });
};

const signOutUser = () => new Promise((resolve, reject) => {
  firebase.auth().signOut().then(resolve).catch(reject);
});

export { signInUser, signOutUser };