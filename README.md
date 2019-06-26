# Mobile App to interface with [Component Database](https://github.com/AdvancedPhotonSource/ComponentDB)
This repository contains the mobile app, that works on android and iOS devices, which allows interfacing with Component Database. 
The purpose of the mobile application is to allow user to complete various "quick" tasks instead of needing to use the web portal. 
# Development
Before you can get started with developing the Mobile App you will need to have a deployment of [Component Database](https://github.com/AdvancedPhotonSource/ComponentDB). Please note that you will need a version of CDB 3.8.0 or newer. 

**Prerequistes:**
- Software
  - Visual Studio Community v8.1.2 or newer
- Hardware
  - Android device
  - Zebra TC70x (optional)
  
```
# Navigate to desired development directory
git clone https://github.com/AdvancedPhotonSource/ComponentDB-Mobile.git

cd ComponentDB-Mobile

# Generate client api using the cdb deployment running.
# Example:
sbin/CreateClientApi.sh http://cdbDeplymentMachine/cdb

# Open the project in visual studio
```
