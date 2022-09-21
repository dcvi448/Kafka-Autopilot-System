# The system simulates the position of the cars in circulation.

Each vehicle position is simulated by 1 task. In order to serve the autopilot system in the future.

# How to run
Navigate to 'docker' folder and run: 
    docker-compose -f docker-kafka-compose.yml up -d --build
Navigate to app/nashtech and run:
1. docker build -t vehiclefaking -f Dockerfile-VehicleFaking .
2. docker build -t trafficmanagementsystem -f Dockerfile-Traffic-Management-System .
3. docker run -it --network host vehiclefaking
4. docker run -it --network host trafficmanagementsystem