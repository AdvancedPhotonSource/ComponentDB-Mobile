#!/bin/bash

# Copyright (c) UChicago Argonne, LLC. All rights reserved.
# See LICENSE file.

#
# Script used to generate the required API client from the running cdb server instance
#
# Usage:
#
# $0 CDB_BASE_PATH
#

MY_DIR=`dirname $0` && cd $MY_DIR && MY_DIR=`pwd`
ROOT_DIR=$MY_DIR/..

OPEN_API_VERSION="3.3.4"
OPEN_API_GENERATOR_JAR="openapi-generator-cli-$OPEN_API_VERSION.jar"
OPEN_API_GENERATOR_JAR_URL="https://repo1.maven.org/maven2/org/openapitools/openapi-generator-cli/$OPEN_API_VERSION/$OPEN_API_GENERATOR_JAR" 

GEN_CONFIG_FILE_PATH=$MY_DIR/ClientApiConfig.yml
GEN_OUT_DIR="c.net"

if [ -z "$1" ]; then
    echo "Please specify CDB_BASE_PATH";
    echo "Usage: $0 CDB_BASE_PATH"
    exit 1; 
fi
CDB_OPENAPI_YML_PATH="api/openapi.yaml"
CDB_OPENAPI_YML_URL="$1/$CDB_OPENAPI_YML_PATH"

cd $ROOT_DIR

rm -rf ApiClient
mkdir ApiClient

cd ApiClient

curl -O $OPEN_API_GENERATOR_JAR_URL

java -jar $OPEN_API_GENERATOR_JAR  generate -i "$CDB_OPENAPI_YML_URL" -g csharp -o $GEN_OUT_DIR -c $GEN_CONFIG_FILE_PATH

cd $GEN_OUT_DIR

sh build.sh