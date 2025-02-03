# **ChessCore** ♟️  

**ChessCore** is a chess engine that implements the **Minimax algorithm with Alpha-Beta pruning**. It supports parallel execution for improved performance.  

## **Features** 🚀  
✅ Chess engine with **Minimax + Alpha-Beta pruning**  
✅ Supports **parallel computation** for better performance  
✅ Can be **Dockerized** and run in a container  
✅ Includes **pre-configured scripts** for building and running  

---

## **Getting Started** 🛠  

## **1️ Clone the Repository**  

git clone https://github.com/val15/ChessCoreSrc.git
cd ChessCoreSrc


## **2️ Build the Docker Image 🐳**
### On Windows
DockerBuild.bat

### On Linux/macOS
chmod +x DockerBuild.sh
./DockerBuild.sh


## **3 Run the Chess Engine in Docker**
### On Windows
DockerComposeUp.bat

### On Linux/macOS
chmod +x DockerComposeUp.sh
./DockerComposeUp.sh