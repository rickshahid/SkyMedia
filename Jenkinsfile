pipeline {
  agent any
  stages {
    stage('Azure Pipeline') {
      parallel {
        stage('Packer') {
          steps {
            echo 'Packer'
          }
        }

        stage('Terraform') {
          steps {
            echo 'Terraform'
          }
        }

      }
    }

  }
}