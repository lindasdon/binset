class binset:
    nil=True
    one=None
    two=None
    def __init__(self, in1=None, in2=None):
        if(in1 is None):
            self.one=None
            self.two=None
        elif(isinstance(in1,binset)):
            if(isinstance(in2,binset)):
                self.one=binset(in1)
                self.two=binset(in2)
            elif(in1.isEmpty()):
                self.one=None
                self.two=None
            elif(in1.isOne()):
                self.one=binset()
                self.two=binset()
            else:
                self.one=binset(in1.one)
                self.two=binset(in1.two)
        elif(isinstance(in1,int)):
            if(in1==0):
                self.one=None
                self.two=None
            elif(in1==1):
                self.one=binset()
                self.two=binset()
            else:
                self.one=binset(in1//2 + in1%2)
                self.two=binset(in1//2)
    def isEmpty(self):
        return (self.one is None) and (self.two is None)
    def isOne(self):
        if(self.isEmpty()):
            return False
        return self.one.isEmpty() # and self.two.isEmpty()
    def count(self):
        if(self.isEmpty()):
            return 0
        elif(self.isOne()):
            return 1
        else:
            return self.one.count() + self.two.count()
    def __eq__(self,other):
        if(self is other):
            return true
        if(type(self)!=type(other)):
            return False
        if(self.isEmpty()):
            return other.isEmpty()
        if(other.isEmpty()):
            return False
        if(self.isOne()):
            return other.isOne()
        if(other.isOne()):
            return False
        return (self.one==other.one) and \
               (self.two==other.two)
    def __and__(self,other):
        if not isinstance(other,binset):
            return self
        if(self.isEmpty() or other.isEmpty()):
            return binset()
        if(self.isOne()):
            if(other.isOne()):
                return binset(1)
            return self & other.one
        if(other.isOne()):
            return self.one & other
        return binset(self.one & other.one, \
                      self.two & other.two)
    def contains(self,other):
        return (self & other)==other
    def __xor__(self,other):
        if not isinstance(other,binset):
            return self
        if(self.isEmpty()):
           return binset(other)
        if(other.isEmpty()):
            return binset(self)
        if(self.isOne()):
            if(other.isOne()):
                return binset()
            return binset(self ^ other.one,other.two)
        if(other.isOne()):
            return binset(self.one ^ other,self.two)
        return binset(self.one ^ other.one, \
                      self.two ^ other.two)
    def __or__(self,other):
        if not isinstance(other,binset):
            return self
        if(self.isEmpty()):
            return binset(other)
        if(other.isEmpty()):
            return binset(self)
        if(self.isOne()):
            if(other.isOne()):
                return binset(1)
            return binset(self | other.one,other.two)
        if(other.isOne()):
            return binset(self.one | other,self.two)
        return binset(self.one | other.one, self.two | other.two)
    def __sub__(self,other):
        if not isinstance(other,binset):
            return self
        if(self.isEmpty() or other.contains(self)):
            return binset()
        if(other.isEmpty()):
            return binset(self)
        if(self.isOne()):
            return self - other.one
        if(other.isOne()):
            return binset(self.one-other,self.two)
        return binset(self.one - other.one, \
                      self.two - other.two)
    def __add__(self,other):
        if not isinstance(other,binset):
            return self
        if(self.isEmpty()):
            return binset(other)
        if(other.isEmpty()):
            return binset(self)
        if(self.isOne()):
            if(other.isOne()):
                return binset(2)
            return binset(self + other.one,other.two)
        if(other.isOne()):
            return binset(self.one + other,self.two)
        return binset(self.one + other.one, \
                      self.two + other.two)
    def __mul__(self,other):
        if not isinstance(other,binset):
            return self
        if(self.isEmpty() or other.isEmpty()):
            return binset()
        if(self.isOne()):
            return binset(other)
        if(other.isOne()):
            return binset(self)
        return binset(self.one * other, \
                      self.two * other)
    def __str__(self):
        if(self.isEmpty()):
            return '0'
        elif(self.isOne()):
            return '1'
        else:
            return self.one.__str__()+':'+self.two.__str__()

class binarray(binset):
    def __init__(self,size:int=2,T:type=binset):
        self.size=size if size>2 else 2
        self.T=T if issubclass(T,binset) else binset
    def __getitem__(self,ix:int):
        if((ix<0) or (ix>=self.size)):
            raise IndexError()
        if(self.size==2):
            if(ix==0):
                return self.one if self.one else self.T()
            return self.two if self.two else self.T()
        if((self.size==3) and (ix==2)):
            return self.two if self.two else self.T()
        if(ix<(self.size+1)/2):
            if isinstance(self.one,binarray):
                return self.one[ix]
            return self.T()
        if isinstance(self.two,binarray):
            return self.two[ix-(self.size + 1)/2]
        return self.T()
    def __setitem__(self,ix:int,value):
        if((ix<0) or (ix>=self.size)):
            raise IndexError()
        if(self.size==2):
            if(ix==0):
                self.one=value
                if not self.two:
                    self.two=self.T()
            else:
                self.two=value
                if not self.one:
                    self.one=self.T()
            return
        if((self.size==3) and (ix==2)):
            self.two=value
            if not self.one:
                self.one=self.T()
            return
        if(ix<(self.size + 1)/2):
            if not isinstance(self.one,binarray):
                self.one=binarray((self.size+1)/2,self.T)
            self.one[ix]=value
            if not self.two:
                self.two=self.T()
        else:
            if not isinstance(self.two,binarray):
                self.two=binarray(self.size/2,self.T)
            self.two[ix-(self.size+1)/2]=value
            if not self.one:
                self.one=self.T()
    def len(self):
        if(self.isOne()):
            return 0
        ct=0
        if(isinstance(self.one,binarray)):
            ct+=self.one.len()
        elif(self.one and not (self.one.isEmpty())):
            ct+=1
        if(isinstance(self.two,binarray)):
            ct+=self.two.len()
        elif(self.two and not (self.two.isEmpty())):
            ct+=1
        return ct

