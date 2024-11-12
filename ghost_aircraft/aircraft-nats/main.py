import asyncio
import math
import time
import nats

# Constants
METERS_PER_DEGREE = 111111
RADIUS_FEET = 100
HEIGHT_OFFSET = 500
NORTH_OFFSET = 500
PERIOD = 30

def get_user_position():
    lat = float(input("Enter user latitude: "))
    lon = float(input("Enter user longitude: "))
    alt = float(input("Enter user altitude (feet): "))
    print(f"\nUser position: lat={lat}, lon={lon}, alt={alt}\n")
    return lat, lon, alt

def calc_drone_position(user_lat, user_lon, user_alt, start_time):
    elapsed = time.time() - start_time
    angle = (elapsed % PERIOD) * (2 * math.pi / PERIOD)
    
    radius_deg = RADIUS_FEET / METERS_PER_DEGREE / 3.28084
    north_offset_deg = NORTH_OFFSET / METERS_PER_DEGREE / 3.28084
    
    drone_lat = user_lat + north_offset_deg + (radius_deg * math.sin(angle))
    drone_lon = user_lon + (radius_deg * math.cos(angle))
    drone_alt = user_alt + HEIGHT_OFFSET
    
    return drone_lat, drone_lon, drone_alt

async def publish_positions(nc, user_pos, drone_pos):
    user_lat, user_lon, user_alt = user_pos
    drone_lat, drone_lon, drone_alt = drone_pos
    
    # Publish user position
    await nc.publish("drone.ground_station_1.user_1.measurement.latitude", str(user_lat).encode())
    await nc.publish("drone.ground_station_1.user_1.measurement.longitude", str(user_lon).encode())
    await nc.publish("drone.ground_station_1.user_1.measurement.altitude", str(user_alt).encode())
    
    # Publish drone position
    await nc.publish("drone.ground_station_1.drone_1.measurement.latitude", str(drone_lat).encode())
    await nc.publish("drone.ground_station_1.drone_1.measurement.longitude", str(drone_lon).encode())
    await nc.publish("drone.ground_station_1.drone_1.measurement.altitude", str(drone_alt).encode())

async def main():
    user_pos = get_user_position()
    start_time = time.time()
    nc = await nats.connect("100.78.13.114:4222")
    
    while True:
        drone_pos = calc_drone_position(*user_pos, start_time)
        await publish_positions(nc, user_pos, drone_pos)
        
        print(f"User: lat={user_pos[0]}, lon={user_pos[1]}, alt={user_pos[2]}")
        print(f"Drone: lat={drone_pos[0]}, lon={drone_pos[1]}, alt={drone_pos[2]}\n")
        
        await asyncio.sleep(1)

if __name__ == "__main__":
    asyncio.run(main())