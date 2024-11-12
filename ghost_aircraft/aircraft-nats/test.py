from main import calc_drone_position, get_user_position, publish_positions
import nats
from unittest.mock import patch

def test_calc_drone_position():
    user_lat = 40.0
    user_lon = -74.0
    user_alt = 1000
    start_time = 0
    
    drone_lat, drone_lon, drone_alt = calc_drone_position(user_lat, user_lon, user_alt, start_time)
    
    assert drone_lat > user_lat  # Due to NORTH_OFFSET
    assert abs(drone_lon - user_lon) < 1  # Should be close due to circular pattern
    assert drone_alt == user_alt + 500  # HEIGHT_OFFSET

def test_get_user_position():
    # Mock user inputs
    test_inputs = ['40.0', '-74.0', '1000']
    with patch('builtins.input', side_effect=test_inputs):
        lat, lon, alt = get_user_position()
        
    assert lat == 40.0
    assert lon == -74.0
    assert alt == 1000.0

async def test_publish_positions():
    # Real NATS test
    nc = await nats.connect("100.78.13.114:4222")
    user_pos = (40.0, -74.0, 1000)
    drone_pos = (40.1, -74.1, 1500)
    
    try:
        await publish_positions(nc, user_pos, drone_pos)
        await nc.close()
    except Exception as e:
        await nc.close()
        raise e

def main():
    print("Running tests...")
    
    # Run sync tests
    test_calc_drone_position()
    test_get_user_position()
    
    # Run async test
    import asyncio
    asyncio.run(test_publish_positions())
    
    print("All tests passed!")

if __name__ == "__main__":
    main()
