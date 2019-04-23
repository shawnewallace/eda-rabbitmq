FROM alpine:3.4
MAINTAINER Shawn Wallace <shawn.wallace@centricconsulting.com>

RUN apk update
RUN apk upgrade
RUN apk add curl wget bash

RUN apk add ruby ruby-bundler

RUN rm -rf /var/cache/apk/*

RUN mkdir /usr/app
WORKDIR /usr/app

RUN gem install bundler --no-rdoc --no-ri
COPY Gemfile /usr/app
COPY Gemfile.lock /usr/app
RUN bundle install

copy ./app/customer_importer.rb /usr/app