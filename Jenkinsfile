pipeline {
  agent any
  stages {
    stage('Init') {
      steps {
        echo 'Init'
        sh 'terraform init'
      }
    }

    stage('Plan') {
      steps {
        echo 'Plan'
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