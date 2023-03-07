# The system simulates the position of the cars in circulation (Docker + .NET6 + Kafka)

Each vehicle position is simulated by 1 task. In order to serve the autopilot system in the future.

# How to run
Navigate to 'docker' folder and run: 

    docker-compose -f docker-kafka-compose.yml up -d --build

Navigate to app/nashtech and run:

    docker build -t vehiclefaking -f Dockerfile-VehicleFaking .
    
    docker build -t trafficmanagementsystem -f Dockerfile-Traffic-Management-System .
    
    docker run -it --network host vehiclefaking
    
    docker run -it --network host trafficmanagementsystem
