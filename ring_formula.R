rin <- 2
rout <- 5
n <- 1000

x <- runif(n, -rout, rout);
y <- array(NA, dim=n)

for (i in 1:n) {
  if (abs(x[i]) <= rin)
  {
    h <- sqrt(rin^2-x[i]^2)
    y[i] <- runif(1, h, h+(rout-rin))
    y[i] <- sample(c(y[i], -y[i]), 1)
  }
  else
  {
    h <- sqrt(rout^2-x[i]^2)
    y[i] <- runif(1, 0, h)
    y[i] <- sample(c(y[i], -y[i]), 1)
  }
}

plot(x,y, xlim=c(-5,5), ylim=c(-5,5))
