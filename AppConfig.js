 import { initializeApp } from "https://www.gstatic.com/firebasejs/10.1.0/firebase-app.js";
 import { getAnalytics } from "https://www.gstatic.com/firebasejs/10.1.0/firebase-analytics.js";

 const firebaseConfig = {
   apiKey: "AIzaSyDZHW1uGfE7K2yz-S5TQegMQBG8cXDGSt8",
   authDomain: "factorio-blueprint--maker.firebaseapp.com",
   databaseURL: "https://factorio-blueprint--maker-default-rtdb.firebaseio.com",
   projectId: "factorio-blueprint--maker",
   storageBucket: "factorio-blueprint--maker.appspot.com",
   messagingSenderId: "559933327660",
   appId: "1:559933327660:web:349432bbb2b8a8565d8489",
   measurementId: "G-15Y1FM8VM8"
 };

 // Initialize Firebase
 const app = initializeApp(firebaseConfig);
 const analytics = getAnalytics(app);

export { app };

