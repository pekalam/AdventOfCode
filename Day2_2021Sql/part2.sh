#!/bin/bash

./generate_sqlfile.sh

sqlcmd -S ".\SQLEXPRESS" -i ./part2.sql