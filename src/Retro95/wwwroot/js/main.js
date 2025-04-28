// 8EGC77F8feUeiOu07LxwWOBa2tfPpasO
import Giphy from './giphy/index.js'

window.Giphy = Giphy

window.addEventListener("load", function () {
  updateClock();
  startClock();
});

function startClock() {
  const now = new Date();
  setTimeout(function () {
    updateClock();
    setInterval(() => updateClock(), 60 * 1000);
  }, (60 - now.getSeconds()) * 1000);
}

function updateClock() {
  const now = new Date();

  let hour = now.getHours();
  const meridiem = hour >= 12 ? "PM" : "AM";
  hour = hour % 12 || 12;

  let minute = now.getMinutes();
  minute = minute < 10 ? `0${minute}` : minute.toString(10);

  document.getElementById("clock").textContent = `${hour}:${minute} ${meridiem}`;
}
