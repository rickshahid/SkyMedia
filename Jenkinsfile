pipeline {
  agent any
  stages {
    stage('Init') {
      when {
        branch 'PR-*'
      }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform init -input=false -backend-config=backend.tf
'''
      }
    }

    stage('Plan') {
      when {
        branch 'PR-*'
      }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform plan -input=false -state=backend.tf -out=plan.tf
'''
        input 'Terraform Plan Approved?'
      }
    }

    stage('Apply') {
      when {
        branch 'PR-*'
      }
      steps {
        sh '''cd $TERRAFORM_CONFIG_DIRECTORY

terraform apply -input=false plan.tf
'''
      }
    }

  }
}
