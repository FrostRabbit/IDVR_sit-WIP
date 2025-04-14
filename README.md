# VR Movement Mechanisms Demo

This Unity VR project demonstrates two types of movement mechanics: **Running** and **Flying**, designed with intuitive physical actions using a VR headset and controllers while **standing in place**.

---

## 1. Running

### Usage
- The player runs by moving their head up and down (detected through the VR headset's Y-axis).
- A **running count** is triggered when the headset moves above an upper threshold and then below a lower threshold.
- The **running speed** is calculated based on the average interval between running counts.

### Advantages
- The player can **control the running speed** naturally.
- **No controller** is required – only the headset movement is used.

### Disadvantages
- Thresholds may vary depending on each individual’s head height and movement.
- The **initial headset position** must be updated frequently to determine current thresholds.

### Design Inspiration
This mechanism is based on natural intuition and motion.

---

## 2. Flying

### Usage
- The player jumps and waves **both hands** (controllers) to fly.
- The **frequency** and **velocity** of waving determine flying **speed** and **height**.
- Motion is captured by checking controller velocity over a threshold.

### Advantages
- Player can **control flying height and speed**.
- Works in both **standing** and **sitting** positions.

### Disadvantages
- Requires **two controllers**.
- Waving both hands continuously can be **tiring** over time.

### Design Inspiration
This mechanism was inspired by **swimming behavior** seen in WIP example videos.

---

## Demo Video

Watch the video demonstration here:  
[Click to watch](https://drive.google.com/file/d/1ZJQ6wMgbDbq3FZ2o2DwReFJMqnh6xWT2/view?usp=sharing)
