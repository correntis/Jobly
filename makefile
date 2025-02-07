.PHONY: env

env:
	docker-compose up --build

services:
	docker-compose -f docker-compose-services.yaml up --build