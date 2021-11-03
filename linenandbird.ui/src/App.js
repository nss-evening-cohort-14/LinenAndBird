import { useEffect, useState } from 'react';
import './App.css';
import BirdList from './components/bird/BirdList';
import {getAllBirds} from "./helpers/data/birdData";
import {signInUser} from './helpers/auth';
import firebase from 'firebase/compat/app';
import 'firebase/compat/auth';
import 'firebase/compat/firestore';

function App() {

  const [birds, setBirds] = useState([]);
  const [user, setUser] = useState({});

  useEffect(() => getAllBirds().then(setBirds), []);
  useEffect(() => {
    firebase.auth().onAuthStateChanged((user) => {
      if (user) {             
        
        //store the token for later   
        user.getIdToken().then((token) => sessionStorage.setItem("token", token));
        
        setUser(user);
      } else {
        setUser(false);
      }
    });
  }, []); 

  return (
    <div className="App">

      <div className="d-flex justify-content-center">
        <button className="signin-button google-logo" onClick={signInUser}>
          <i className="fas fa-sign-out-alt"></i> Sign In
        </button>
      </div>

      <BirdList birds={birds}/>
    </div>
  );
}

export default App;
