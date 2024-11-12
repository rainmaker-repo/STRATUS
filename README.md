# STRATUS
Aircraft Tracking and Visualization System

## Overview
STRATUS is a real-time aircraft tracking and visualization system that connects a Unity-based 3D visualization environment with live position data via NATS messaging.

## Components

### Position Simulator (main.py)
Simulates aircraft movement by:
- Getting user position input
- Calculating drone position in a circular pattern relative to user
- Publishing positions to NATS server topics:
  - User position: `drone.ground_station_1.user_1.measurement.*`
  - Drone position: `drone.ground_station_1.drone_1.measurement.*`

### Test Suite (test.py) 
Validates core functionality:
- Position calculation algorithms
- User input handling
- NATS connectivity and message publishing

### Unity Visualization (TBD)
- 3D visualization environment showing user and drone positions
- Subscribes to NATS topics for position updates
- Real-time rendering of movement

## For Unity Developer
- Create a new `/unity` directory in this repo
- Commit and push Unity project files and updates regularly
- Subscribe to NATS topics to receive position data
- Implement smooth position updates in visualization

## Getting Started
1. Install dependencies:
```bash
pixi install
```

2. Run position simulator:
```bash
python main.py
```

3. Run tests:
```bash
python test.py
```

## NATS Server
- Host: 100.78.13.114:4222
- No authentication required for development

## Constants
- Drone maintains 500ft height offset above user
- Circular pattern with 100ft radius
- 30 second rotation period

## Message Format
### Position Messages
All position messages follow this JSON format:
```json
{
    "timestamp": "2024-11-12T15:04:05.999Z",
    "position": {
        "latitude": 37.7749,
        "longitude": -122.4194,
        "altitude": 500.0
    },
    "metadata": {
        "id": "user_1",
        "type": "ground_station",
        "status": "active"
    }
}
```
## Troubleshooting
### Common Issues
1. NATS Connection Failure
   - Verify NATS server is running
   - Check network connectivity
   - Confirm port 4222 is open

2. Position Updates Not Showing
   - Check NATS topic subscription
   - Verify message format
   - Look for validation errors in logs
