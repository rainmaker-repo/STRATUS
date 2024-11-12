import asyncio
import math
import time
from dataclasses import dataclass
import nats

# ~111,111 meters per degree at equator
METERS_PER_DEGREE_LAT = 111111
# Varies w/ latitude but close enough for small distances
METERS_PER_DEGREE_LON = 111111

@dataclass
class Position:
    lat: float
    lon: float
    alt: float

class DroneSimulator:
    def __init__(self, user_pos: Position):
        self.user_pos = user_pos
        self.radius_feet = 100
        self.height_offset = 500  # feet above user
        self.north_offset = 500   # feet north of user
        self.period = 30          # seconds per revolution
        self.start_time = time.time()

    def calc_drone_position(self) -> Position:
        """Calculate drone position based on circular flight path"""
        elapsed = time.time() - self.start_time
        angle = (elapsed % self.period) * (2 * math.pi / self.period)
        
        # Convert feet to degrees for lat/lon
        radius_deg = self.radius_feet / METERS_PER_DEGREE_LAT / 3.28084
        north_offset_deg = self.north_offset / METERS_PER_DEGREE_LAT / 3.28084
        
        # Circle centered north of user
        lat = self.user_pos.lat + north_offset_deg + (radius_deg * math.sin(angle))
        lon = self.user_pos.lon + (radius_deg * math.cos(angle))
        alt = self.user_pos.alt + self.height_offset

        return Position(lat, lon, alt)

async def main():
    # Get user input
    user_lat = float(input("Enter user latitude: "))
    user_lon = float(input("Enter user longitude: "))
    user_alt = float(input("Enter user altitude (feet): "))
    user_pos = Position(user_lat, user_lon, user_alt)
    
    # Init simulator
    sim = DroneSimulator(user_pos)
    
    # Connect to remote NATS
    nc = await nats.connect("100.78.13.114:4222")
    
    while True:
        drone_pos = sim.calc_drone_position()
        
        # Publish user position
        await nc.publish("drone.ground_station_1.user_1.measurement.latitude", str(user_pos.lat).encode())
        await nc.publish("drone.ground_station_1.user_1.measurement.longitude", str(user_pos.lon).encode())
        await nc.publish("drone.ground_station_1.user_1.measurement.altitude", str(user_pos.alt).encode())
        
        # Publish drone position
        await nc.publish("drone.ground_station_1.drone_1.measurement.latitude", str(drone_pos.lat).encode())
        await nc.publish("drone.ground_station_1.drone_1.measurement.longitude", str(drone_pos.lon).encode())
        await nc.publish("drone.ground_station_1.drone_1.measurement.altitude", str(drone_pos.alt).encode())
        
        # Print to console
        print(f"User: {user_pos}")
        print(f"Drone: {drone_pos}\n")
        
        await asyncio.sleep(1)

if __name__ == "__main__":
    asyncio.run(main())