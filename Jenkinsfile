pipeline {
  agent any
  stages {
    stage('Init') {
      steps {
        sh '''cd /terraform/examples/vfxt/3-filers/
pwd
ls
terraform init'''
      }
    }

    stage('Plan') {
      steps {
        sh '''terraform plan
'''
      }
    }

    stage('Review') {
      steps {
        echo 'Review'
      }
    }

    stage('Apply') {
      steps {
        echo 'Apply'
      }
    }

  }
}